namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Deployment;

using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance deploy compile")]
public class CreateDeploymentCommand : BaseSessionCommand
{
    private readonly IInstanceClientManager instances;

    public CreateDeploymentCommand(IInstanceClientManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        await client.DreamMaker.Compile(context.CancellationToken);
    }
}
