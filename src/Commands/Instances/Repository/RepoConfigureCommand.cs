namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;

[Command("instance repo config", Description = "Configures the repository settings of an instance.")]
public sealed class RepoConfigureCommand : BaseInstanceClientCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    [CommandOption("committer-name", Description = "The name used for authoring merge commits.")]
    public string? CommitterName { get; init; }

    [CommandOption("committer-email", Description = "The email used for authoring merge commits.")]
    public string? CommitterEmail { get; init; }

    [CommandOption("update-submodules", Description = "Whether submodules should be updated or not.")]
    public bool? UpdateSubmodules { get; init; }

    [CommandOption("auto-sync", Description = "Whether synchronizations should occur when auto updates are ran.")]
    public bool? AutoUpdatesSynchronize { get; init; }

    [CommandOption("tm-comments", Description = "Whether test merges should create comments under the test-merged PR.")]
    public bool? PostTestMergeComments { get; init; }

    [CommandOption("keep-tms", Description = "Whether auto updates should keep active test merges.")]
    public bool? KeepTestMerges { get; init; }

    public RepoConfigureCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var instanceClient = await this.RequestInstanceClient(this.Instance, console);

        await instanceClient.Repository.Update(
            new RepositoryUpdateRequest
            {
                CommitterName = this.CommitterName,
                CommitterEmail = this.CommitterEmail,
                UpdateSubmodules = this.UpdateSubmodules,
                AutoUpdatesSynchronize = this.AutoUpdatesSynchronize,
                PostTestMergeComment = this.PostTestMergeComments,
                AutoUpdatesKeepTestMerges = this.KeepTestMerges
            }, console.RegisterCancellationHandler());
    }
}
