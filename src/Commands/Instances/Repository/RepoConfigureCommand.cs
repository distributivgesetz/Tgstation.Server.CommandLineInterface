namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;

[Command("instance repo config")]
public sealed class RepoConfigureCommand : BaseInstanceClientCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    [CommandOption("ref")]
    public string? Reference { get; init; }

    [CommandOption("committer-name")]
    public string? CommitterName { get; init; }

    [CommandOption("committer-email")]
    public string? CommitterEmail { get; init; }

    [CommandOption("update-submodules")]
    public bool? UpdateSubmodules { get; init; }

    [CommandOption("auto-sync")]
    public bool? AutoUpdatesSynchronize { get; init; }

    [CommandOption("tm-comments")]
    public bool? PostTestMergeComments { get; init; }

    [CommandOption("keep-tms")]
    public bool? KeepTestMerges { get; init; }

    public RepoConfigureCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var instanceClient = await this.RequestInstanceClient(this.Instance, console);

        await instanceClient.Repository.Update(new RepositoryUpdateRequest
        {
            Reference = this.Reference,
            CommitterName = this.CommitterName,
            CommitterEmail = this.CommitterEmail,
            UpdateSubmodules = this.UpdateSubmodules,
            AutoUpdatesSynchronize = this.AutoUpdatesSynchronize,
            PostTestMergeComment = this.PostTestMergeComments,
            AutoUpdatesKeepTestMerges = this.KeepTestMerges
        }, console.RegisterCancellationHandler());
    }
}
