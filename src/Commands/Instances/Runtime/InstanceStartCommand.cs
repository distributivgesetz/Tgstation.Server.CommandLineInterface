namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Runtime;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;
using Sessions;

[Command("instance start")]
public class InstanceStartCommand : BaseSessionCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    public InstanceStartCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);
        var token = console.RegisterCancellationHandler();
        var updateRequest = new InstanceUpdateRequest {Online = true, Id = this.Instance};
        await client.Instances.Update(updateRequest, token);
    }
}
