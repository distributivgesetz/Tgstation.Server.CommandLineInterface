namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Runtime;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance start", Description = "Starts an instance.")]
public sealed class InstanceStartCommand : BaseInstanceClientCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    public InstanceStartCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var token = context.CancellationToken;
        var client = await this.Sessions.ResumeSession(token);
        var updateRequest =
            new InstanceUpdateRequest { Online = true, Id = await this.SelectInstanceId(this.Instance, token) };
        await client.Instances.Update(updateRequest, token);
    }
}
