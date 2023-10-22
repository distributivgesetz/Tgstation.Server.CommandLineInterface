namespace Tgstation.Server.CommandLineInterface.Commands.Instances;

using Client.Components;
using Models;
using Services;
using Sessions;

public abstract class BaseInstanceClientCommand : BaseSessionCommand
{
    protected BaseInstanceClientCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected async ValueTask<IInstanceClient> RequestInstanceClient(InstanceSelector target, CancellationToken token)
    {
        var client = await this.Sessions.ResumeSession(token);
        return client.Instances.CreateClient(target);
    }
}
