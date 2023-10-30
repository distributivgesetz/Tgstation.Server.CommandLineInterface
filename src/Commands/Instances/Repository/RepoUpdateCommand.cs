namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance repo update", Description = "Runs a pull from origin on an instance.")]
public class RepoUpdateCommand : BaseSessionCommand
{
    private readonly IInstanceManager instances;

    public RepoUpdateCommand(IInstanceManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    [CommandOption("ref", 'r', Description = "The reference the repo should pull from.")]
    public string? Reference { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        await client.Repository.Update(
            new RepositoryUpdateRequest {UpdateFromOrigin = true, Reference = this.Reference},
            context.CancellationToken);
    }
}
