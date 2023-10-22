namespace Tgstation.Server.CommandLineInterface.Commands.Instances;

using Client.Components;
using CliFx.Exceptions;
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

        if (target.Id != null)
        {
            return client.Instances.CreateClient(target);
        }

        var res = (await client.Instances.List(null, token)).FirstOrDefault(i => i.Name == target.Name);

        if (res == null)
        {
            throw new CommandException("Instance not found");
        }

        target = new InstanceSelector(res.Id!.Value);

        return client.Instances.CreateClient(target);
    }
}
