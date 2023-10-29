namespace Tgstation.Server.CommandLineInterface.Commands.Instances;

using System.Globalization;
using System.Text;
using CliFx.Attributes;
using Extensions;
using Middlewares;
using Services;
using Sessions;

[Command("instance list", Description = "Lists all instances that are visible to the current user.")]
public sealed class ListInstancesCommand : BaseSessionCommand
{
    private readonly IRemoteRegistry remotes;

    public ListInstancesCommand(ISessionManager sessions, IRemoteRegistry remotes) : base(sessions) =>
        this.remotes = remotes;

    [CommandOption("brief", Description = "Display only instance names and IDs.")]
    public bool Brief { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.Sessions.ResumeSession(context.CancellationToken);

        var token = context.CancellationToken;

        var instances = await client.Instances.List(null, token);

        var output = new StringBuilder();

        output.Append(CultureInfo.InvariantCulture, $"All instances for {this.remotes.GetCurrentRemote().Host} " +
            $"(visible to current user):\n");

        foreach (var instance in instances)
        {
            output.AppendLine();
            output.AppendLine(CultureInfo.InvariantCulture,
                $"Name: {instance.Name} ({instance.Id}) {(!instance.Accessible ? " [no access]" : "")}");

            output.AppendLine(CultureInfo.InvariantCulture, $"  Currently Online: {instance.Online}");

            if (this.Brief)
            {
                continue;
            }

            output.AppendLine(CultureInfo.InvariantCulture, $"  Path On Disk: {instance.Path}");

            output.AppendLine(CultureInfo.InvariantCulture, $"  Configuration Allowed: {instance.ConfigurationType}");
            output.AppendLine(CultureInfo.InvariantCulture, $"  Auto Update Interval: " +
                $"{(instance.AutoUpdateInterval == 0 ? "Disabled" : $"{instance.AutoUpdateInterval} minutes")}");

            output.AppendLine(CultureInfo.InvariantCulture, $"  Maximum Chat Bots: {instance.ChatBotLimit}");
            output.AppendLine(CultureInfo.InvariantCulture,
                $"  Is Currently Moving On Disk: {instance.MoveJob != null}");
        }

        await context.Console.WriteAsync(output, token);
    }
}
