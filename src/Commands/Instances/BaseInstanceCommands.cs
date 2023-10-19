namespace Tgstation.Server.CommandLineInterface.Commands.Instances;

using Client.Components;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;
using Sessions;

public abstract class BaseInstanceCommand : BaseSessionCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    protected BaseInstanceCommand(ISessionManager sessions) : base(sessions)
    {
    }
}

public abstract class BaseInstanceClientCommand : BaseInstanceCommand
{
    protected BaseInstanceClientCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected async ValueTask<IInstanceClient> RequestInstanceClient(IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);
        return client.Instances.CreateClient(this.Instance);
    }
}
