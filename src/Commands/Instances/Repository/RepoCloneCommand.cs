namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Services;

[Command("instance repo clone")]
public class RepoCloneCommand : BaseInstanceClientCommand
{
    [CommandParameter(1)] public required Uri RepositoryUrl { get; init; }

    [CommandParameter(2)] public required string Ref { get; init; }

    public RepoCloneCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var instanceClient = await this.RequestInstanceClient(console);
        await instanceClient.Repository.Clone(
            new RepositoryCreateRequest {Origin = this.RepositoryUrl, Reference = this.Ref},
            console.RegisterCancellationHandler());
    }
}
