namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;

[Command("instance repo clone", Description = "Clones a repository into an instance.")]
public sealed class RepoCloneCommand : BaseInstanceClientCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The target instance.")]
    public required InstanceSelector Instance { get; init; }

    [CommandParameter(1, Description = "The repository URL to clone from.")]
    public required Uri RepositoryUrl { get; init; }

    [CommandOption("ref", 'r', Description = "The reference (branch) to clone.")]
    public string? Ref { get; init; }

    public RepoCloneCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var instanceClient = await this.RequestInstanceClient(this.Instance, console);
        await instanceClient.Repository.Clone(
            new RepositoryCreateRequest { Origin = this.RepositoryUrl, Reference = this.Ref },
            console.RegisterCancellationHandler());
    }
}
