namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Management;

using CliFx.Attributes;
using CliFx.Infrastructure;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("instance delete")]
public class InstanceDeleteCommand : BaseInstanceCommand
{

    public InstanceDeleteCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares)
    {
        middlewares.UseMiddleware<RequestFailHandlerMiddleware>();
        middlewares.UseMiddleware<EnsureCurrentSessionMiddleware>();
        middlewares.UseMiddleware<UserUnauthorizedHandler>();
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);
        await client.Instances.Detach(this.Instance, console.RegisterCancellationHandler());
    }
}
