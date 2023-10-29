namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Models;
using Services;

[Command("instance repo update", Description = "Runs a pull from origin on an instance.")]
public class RepoUpdateCommand : BaseInstanceClientCommand
{
    public RepoUpdateCommand(ISessionManager sessions) : base(sessions)
    {
    }

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    [CommandOption("ref", 'r', Description = "The reference the repo should pull from.")]
    public string? Reference { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.RequestInstanceClient(this.Instance, context.CancellationToken);
        await client.Repository.Update(
            new RepositoryUpdateRequest {UpdateFromOrigin = true, Reference = this.Reference},
            context.CancellationToken);
    }
}
