namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;

[Command("instance repo update")]
public class RepoUpdateCommand : BaseInstanceClientCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    public RepoUpdateCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.RequestInstanceClient(this.Instance, console);
        await client.Repository.Update(new RepositoryUpdateRequest { UpdateFromOrigin = true },
            console.RegisterCancellationHandler());
    }
}
