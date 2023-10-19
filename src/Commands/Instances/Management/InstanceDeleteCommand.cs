namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Management;

using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;
using Sessions;

[Command("instance delete")]
public sealed class InstanceDeleteCommand : BaseSessionCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    public InstanceDeleteCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);
        await client.Instances.Detach(this.Instance, console.RegisterCancellationHandler());
    }
}
