namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Management;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Services;
using Sessions;

[Command("instance create", Description = "Creates or attaches a new instance to the server.")]
public sealed class InstanceCreateCommand : BaseSessionCommand
{
    [CommandParameter(0, Description = "The name of the new instance.")]
    public required string Name { get; init; }

    [CommandParameter(1, Description = "The path of the new or already existing instance.")]
    public required string Path { get; init; }

    public InstanceCreateCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);
        var request = new InstanceCreateRequest
        {
            Name = this.Name,
            Path = this.Path
        };
        await client.Instances.CreateOrAttach(request, console.RegisterCancellationHandler());
    }
}
