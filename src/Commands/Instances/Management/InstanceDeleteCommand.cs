namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Management;

using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;
using Sessions;

[Command("instance detach", Description = "Detaches an instance.")]
public sealed class InstanceDetachCommand : BaseSessionCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    public InstanceDetachCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);
        await client.Instances.Detach(this.Instance, console.RegisterCancellationHandler());
    }
}
