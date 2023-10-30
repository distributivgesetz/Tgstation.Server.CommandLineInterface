namespace Tgstation.Server.CommandLineInterface.Commands.Sessions;

using Middlewares;
using Middlewares.Implementations;

public abstract class BaseSessionCommand : BaseCommand
{
    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares)
    {
        middlewares.UseMiddleware<EnsureCurrentRemoteMiddleware>();
        middlewares.UseMiddleware<RequestFailHandlerMiddleware>();
        middlewares.UseMiddleware<EnsureServerReachableMiddleware>();
        middlewares.UseMiddleware<AutoLoginMiddleware>();
    }
}
