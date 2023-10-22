namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Management;

using CliFx.Attributes;
using Middlewares;
using Models;
using Services;

[Command("instance detach", Description = "Detaches an instance.")]
public sealed class InstanceDetachCommand : BaseInstanceClientCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    public InstanceDetachCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.Sessions.ResumeSession(context.CancellationToken);
        await client.Instances.Detach(await this.SelectInstance(this.Instance, context.CancellationToken), context.CancellationToken);
    }
}
