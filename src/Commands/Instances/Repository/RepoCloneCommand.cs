namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance repo clone", Description = "Clones a repository into an instance.")]
public sealed class RepoCloneCommand : BaseSessionCommand
{
    private readonly IInstanceManager instances;

    public RepoCloneCommand(ISessionManager sessions, IInstanceManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The target instance.")]
    public required InstanceSelector Instance { get; init; }

    [CommandParameter(1, Description = "The repository URL to clone from.")]
    public required Uri RepositoryUrl { get; init; }

    [CommandOption("ref", 'r', Description = "The reference (branch) to clone.")]
    public string? Ref { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var instanceClient = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        await instanceClient.Repository.Clone(
            new RepositoryCreateRequest {Origin = this.RepositoryUrl, Reference = this.Ref},
            context.CancellationToken);
    }
}
