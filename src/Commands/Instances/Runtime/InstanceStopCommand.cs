namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Runtime;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance stop", Description = "Stops an instance.")]
public sealed class InstanceStopCommand : BaseSessionCommand
{
    private readonly ISessionManager sessions;
    private readonly IInstanceManager instances;

    public InstanceStopCommand(ISessionManager sessions, IInstanceManager instances)
    {
        this.sessions = sessions;
        this.instances = instances;
    }

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.sessions.ResumeSession(context.CancellationToken);
        var token = context.CancellationToken;
        var updateRequest =
            new InstanceUpdateRequest {Online = false, Id = await this.instances.SelectInstanceId(this.Instance, token)};
        await client.Instances.Update(updateRequest, token);
    }
}
