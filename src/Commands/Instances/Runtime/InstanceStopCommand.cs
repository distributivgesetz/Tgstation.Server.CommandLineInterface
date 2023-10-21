namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Runtime;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;
using Sessions;

[Command("instance stop", Description = "Stops an instance.")]
public sealed class InstanceStopCommand : BaseSessionCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    public InstanceStopCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);
        var token = console.RegisterCancellationHandler();
        var updateRequest = new InstanceUpdateRequest { Online = false, Id = this.Instance };
        await client.Instances.Update(updateRequest, token);
    }
}
