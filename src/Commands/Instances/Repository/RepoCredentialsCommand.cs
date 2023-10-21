namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;

[Command("instance repo credentials")]
public class RepoCredentialsCommand : BaseInstanceClientCommand
{
    [CommandParameter(0)]
    public required InstanceSelector Instance { get; init; }

    [CommandParameter(1)]
    public required string AccessName { get; init; }

    [CommandParameter(2)]
    public required string AccessPhrase { get; init; }

    public RepoCredentialsCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var instanceClient = await this.RequestInstanceClient(this.Instance, console);
        await instanceClient.Repository.Update(new RepositoryUpdateRequest
        {
            AccessUser = this.AccessName,
            AccessToken = this.AccessPhrase
        }, console.RegisterCancellationHandler());
    }
}
