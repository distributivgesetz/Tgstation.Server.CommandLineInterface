namespace Tgstation.Server.CommandLineInterface.Commands.Sessions;

using Middlewares;
using Middlewares.Implementations;
using Services;

public abstract class BaseSessionCommand : BaseCommand
{
    protected ISessionManager Sessions { get; set; }
    protected BaseSessionCommand(ISessionManager sessions) => this.Sessions = sessions;

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares)
    {
        middlewares.UseMiddleware<EnsureCurrentRemoteMiddleware>();
        middlewares.UseMiddleware<RequestFailHandlerMiddleware>();
        middlewares.UseMiddleware<EnsureServerReachableMiddleware>();
        middlewares.UseMiddleware<AutoLoginMiddleware>();
    }
}
