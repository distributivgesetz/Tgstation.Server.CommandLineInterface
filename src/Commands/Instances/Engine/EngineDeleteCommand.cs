namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Engine;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance engine delete")]
public class EngineDeleteCommand : BaseSessionCommand
{
    private readonly IInstanceManager instances;

    public EngineDeleteCommand(IInstanceManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; set; }

    [CommandParameter(1, Converter = typeof(VersionConverter))]
    public required Version Version { get; set; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        await client.Byond.DeleteVersion(new ByondVersionDeleteRequest {Version = this.Version},
            context.CancellationToken);
    }
}
