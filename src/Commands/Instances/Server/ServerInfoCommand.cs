namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Server;

using System.Globalization;
using System.Text;
using CliFx.Attributes;
using Extensions;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance server")]
public class ServerInfoCommand : BaseSessionCommand
{
    private readonly IInstanceClientManager instances;

    public ServerInfoCommand(IInstanceClientManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        var res = await client.DreamDaemon.Read(context.CancellationToken);

        var output = new StringBuilder();

        output.AppendLine(CultureInfo.InvariantCulture, $"Status: {res.Status.ToString()}");
        output.AppendLine(CultureInfo.InvariantCulture, $"Port: {res.CurrentPort}");
        output.AppendLine(CultureInfo.InvariantCulture, $"Security level: {res.CurrentSecurity.ToString()}");
        output.AppendLine(CultureInfo.InvariantCulture, $"Visibility level: {res.CurrentVisibility.ToString()}");

        // TODO: this needs to be more detailed
        await context.Console.WriteAsync(output.ToString());
    }
}
