namespace Tgstation.Server.CommandLineInterface.Commands.Instances;

using Client.Components;
using CliFx.Infrastructure;
using Models;
using Services;
using Sessions;

public abstract class BaseInstanceClientCommand : BaseSessionCommand
{
    protected BaseInstanceClientCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected async ValueTask<IInstanceClient> RequestInstanceClient(InstanceSelector target, IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);
        return client.Instances.CreateClient(target);
    }
}
