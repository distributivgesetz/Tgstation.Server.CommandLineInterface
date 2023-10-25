namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using Services;

public class EnsureServerReachableMiddleware : ICommandMiddleware
{
    private readonly ITgsClientManager manager;
    private readonly IRemoteRegistry remotes;

    public EnsureServerReachableMiddleware(ITgsClientManager manager, IRemoteRegistry remotes)
    {
        this.manager = manager;
        this.remotes = remotes;
    }

    public async ValueTask HandleCommandAsync(ICommandContext context, PipelineNext nextStep)
    {
        // exception is handled up the chain
        await this.manager.PingServer(this.remotes.GetCurrentRemote().Host);
        await nextStep();
    }
}
