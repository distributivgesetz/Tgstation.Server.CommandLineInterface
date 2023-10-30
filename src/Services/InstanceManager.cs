namespace Tgstation.Server.CommandLineInterface.Services;

using Api.Models;
using Client.Components;
using CliFx.Exceptions;
using Models;

public interface IInstanceManager
{
    ValueTask<IInstanceClient> RequestInstanceClient(InstanceSelector target, CancellationToken token);
    ValueTask<Instance> SelectInstance(InstanceSelector target, CancellationToken token);
    ValueTask<long> SelectInstanceId(InstanceSelector target, CancellationToken token);
}

public class InstanceManager : IInstanceManager
{
    private readonly SessionManager sessionManager;

    public InstanceManager(SessionManager sessionManager) => this.sessionManager = sessionManager;

    public async ValueTask<IInstanceClient> RequestInstanceClient(InstanceSelector target, CancellationToken token)
    {
        var client = await this.sessionManager.ResumeSession(token);
        return client.Instances.CreateClient(await this.SelectInstance(target, token));
    }

    public async ValueTask<Instance> SelectInstance(InstanceSelector target, CancellationToken token)
    {
        var client = await this.sessionManager.ResumeSession(token);

        if (target.Id != null)
        {
            return (Instance)target;
        }

        var res = (await client.Instances.List(null, token)).FirstOrDefault(i =>
            i.Name!.StartsWith(target.Name!, StringComparison.InvariantCulture));

        return res ?? throw new CommandException("Instance not found");
    }

    public async ValueTask<long> SelectInstanceId(InstanceSelector target, CancellationToken token)
    {
        var client = await this.sessionManager.ResumeSession(token);

        if (target.Id != null)
        {
            return target.Id.Value;
        }

        var res = (await client.Instances.List(null, token)).FirstOrDefault(i =>
            i.Name!.StartsWith(target.Name!, StringComparison.InvariantCulture));

        return res?.Id ?? throw new CommandException("Instance not found");
    }
}
