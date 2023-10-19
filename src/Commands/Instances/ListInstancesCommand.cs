namespace Tgstation.Server.CommandLineInterface.Commands.Instances;

using System.Globalization;
using System.Text;
using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using Services;
using Sessions;

[Command("list-instances"), UsedImplicitly]
public sealed class ListInstancesCommand : BaseSessionCommand
{
    private readonly IRemoteRegistry remotes;

    [CommandOption("brief", Description = "Display only instance names and IDs.")]
    public bool Brief { get; init; }

    public ListInstancesCommand(ISessionManager sessions, IRemoteRegistry remotes) : base(sessions) => this.remotes = remotes;

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);

        var token = console.RegisterCancellationHandler();

        var instances = await client.Instances.List(null, token);

        var output = new StringBuilder();

        output.AppendLine(CultureInfo.InvariantCulture, $"All instances for {this.remotes.GetCurrentRemote().Host}:\n");

        foreach (var instance in instances)
        {
            output.AppendLine(CultureInfo.InvariantCulture,
                $"Name: {instance.Name} ({instance.Id}) {(!instance.Accessible ? " [no access]" : "")}");

            output.AppendLine(CultureInfo.InvariantCulture, $"\tCurrently Online: {instance.Online}");

            if (this.Brief)
            {
                continue;
            }

            output.AppendLine(CultureInfo.InvariantCulture, $"  Path On Disk: {instance.Path}");

            output.AppendLine(CultureInfo.InvariantCulture, $"  Configuration Allowed: {instance.ConfigurationType}");
            output.AppendLine(CultureInfo.InvariantCulture, $"  Auto Update Interval: " +
                $"{(instance.AutoUpdateInterval == 0 ? "Disabled" : $"{instance.AutoUpdateInterval} minutes")}");

            output.AppendLine(CultureInfo.InvariantCulture, $"  Maximum Chat Bots: {instance.ChatBotLimit}");
            output.AppendLine(CultureInfo.InvariantCulture, $"  Is Currently Moving On Disk: {instance.MoveJob != null}");
        }

        await console.Output.WriteLineAsync(output, token);
    }
}
