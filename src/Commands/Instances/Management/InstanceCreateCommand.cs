namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Management;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Services;
using Sessions;

[Command("instance attach", Description = "Creates or attaches a new instance to the server.")]
public sealed class InstanceCreateCommand : BaseSessionCommand
{
    public InstanceCreateCommand(ISessionManager sessions) : base(sessions)
    {
    }

    [CommandParameter(0, Description = "The name of the new instance.")]
    public required string Name { get; init; }

    [CommandParameter(1, Description = "The path of the new or already existing instance.")]
    public required string Path { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.Sessions.ResumeSession(context.CancellationToken);
        var request = new InstanceCreateRequest {Name = this.Name, Path = this.Path};
        await client.Instances.CreateOrAttach(request, context.CancellationToken);
    }
}
