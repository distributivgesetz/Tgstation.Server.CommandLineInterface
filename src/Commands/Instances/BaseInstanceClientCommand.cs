namespace Tgstation.Server.CommandLineInterface.Commands.Instances;

using Api.Models;
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
        return client.Instances.CreateClient(await this.SelectInstance(target, token));
    }

    protected async ValueTask<Instance> SelectInstance(InstanceSelector target, CancellationToken token)
    {
        var client = await this.Sessions.ResumeSession(token);

        if (target.Id != null)
        {
            return (Instance) target;
        }

        var res = (await client.Instances.List(null, token)).FirstOrDefault(i =>
            i.Name!.StartsWith(target.Name!, StringComparison.InvariantCulture));

        return res ?? throw new CommandException("Instance not found");
    }

    protected async ValueTask<long> SelectInstanceId(InstanceSelector target, CancellationToken token)
    {
        var client = await this.Sessions.ResumeSession(token);

        if (target.Id != null)
        {
            return target.Id.Value;
        }

        var res = (await client.Instances.List(null, token)).FirstOrDefault(i =>
            i.Name!.StartsWith(target.Name!, StringComparison.InvariantCulture));

        return res?.Id ?? throw new CommandException("Instance not found");
    }
}
