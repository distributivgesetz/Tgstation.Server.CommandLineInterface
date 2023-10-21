namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using Commands;
using Services;

public class RequestFailHandlerMiddleware : ICommandMiddleware
{
    private readonly IRemoteRegistry remotes;

    public RequestFailHandlerMiddleware(IRemoteRegistry remotes) => this.remotes = remotes;

    public async ValueTask HandleCommandAsync(IMiddlewareContext context, PipelineNext nextStep)
    {
        var host = this.remotes.HasCurrentRemote() ? this.remotes.GetCurrentRemote().Host : null;

        await RequestHelpers.TryServerRequestAsync(async () => await nextStep(), host);
    }
}
