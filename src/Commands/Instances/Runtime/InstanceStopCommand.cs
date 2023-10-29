namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Runtime;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Models;
using Services;

[Command("instance stop", Description = "Stops an instance.")]
public sealed class InstanceStopCommand : BaseInstanceClientCommand
{
    public InstanceStopCommand(ISessionManager sessions) : base(sessions)
    {
    }

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.Sessions.ResumeSession(context.CancellationToken);
        var token = context.CancellationToken;
        var updateRequest =
            new InstanceUpdateRequest {Online = false, Id = await this.SelectInstanceId(this.Instance, token)};
        await client.Instances.Update(updateRequest, token);
    }
}
