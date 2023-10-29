namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Models;
using Services;

[Command("instance repo credentials")]
public class RepoCredentialsCommand : BaseInstanceClientCommand
{
    public RepoCredentialsCommand(ISessionManager sessions) : base(sessions)
    {
    }

    [CommandParameter(0)] public required InstanceSelector Instance { get; init; }

    [CommandParameter(1)] public required string AccessName { get; init; }

    [CommandParameter(2)] public required string AccessPhrase { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var instanceClient = await this.RequestInstanceClient(this.Instance, context.CancellationToken);
        await instanceClient.Repository.Update(
            new RepositoryUpdateRequest {AccessUser = this.AccessName, AccessToken = this.AccessPhrase},
            context.CancellationToken);
    }
}
