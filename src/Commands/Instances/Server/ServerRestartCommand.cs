namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Server;

using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance server restart")]
public class ServerRestartCommand : BaseSessionCommand
{
    private readonly IInstanceClientManager instances;

    public ServerRestartCommand(IInstanceClientManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        await client.DreamDaemon.Restart(context.CancellationToken);
    }
}
