namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;

[Command("instance repo update", Description = "Runs a pull from origin on an instance.")]
public class RepoUpdateCommand : BaseInstanceClientCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    [CommandOption("ref", 'r', Description = "The reference the repo should pull from.")]
    public string? Reference { get; init; }

    public RepoUpdateCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.RequestInstanceClient(this.Instance, console);
        await client.Repository.Update(
            new RepositoryUpdateRequest { UpdateFromOrigin = true, Reference = this.Reference },
            console.RegisterCancellationHandler());
    }
}
