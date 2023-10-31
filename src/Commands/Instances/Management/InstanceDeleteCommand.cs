namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Management;

using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance detach", Description = "Detaches an instance.")]
public sealed class InstanceDetachCommand : BaseSessionCommand
{
    private readonly IInstanceClientManager instances;
    private readonly ISessionManager sessions;

    public InstanceDetachCommand(ISessionManager sessions, IInstanceClientManager instances)
    {
        this.sessions = sessions;
        this.instances = instances;
    }

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.sessions.ResumeSession(context.CancellationToken);
        await client.Instances.Detach(await this.instances.SelectInstance(this.Instance, context.CancellationToken),
            context.CancellationToken);
    }
}
