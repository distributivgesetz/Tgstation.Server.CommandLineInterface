namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance repo delete", Description = "Resets an instance's repository.")]
public sealed class RepoDeleteCommand : BaseSessionCommand
{
    private readonly IInstanceClientManager instances;

    public RepoDeleteCommand(IInstanceClientManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var instanceClient = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        await instanceClient.Repository.Delete(context.CancellationToken);
    }
}
