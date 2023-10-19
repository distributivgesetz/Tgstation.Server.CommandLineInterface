namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Runtime;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("instance start")]
public class InstanceStartCommand : BaseInstanceCommand
{
    public InstanceStartCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares)
    {
        middlewares.UseMiddleware<EnsureCurrentSessionMiddleware>();
        middlewares.UseMiddleware<UserUnauthorizedHandler>();
        middlewares.UseMiddleware<RequestFailHandlerMiddleware>();
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.Sessions.ResumeSessionOrReprompt(console);
        var token = console.RegisterCancellationHandler();
        var updateRequest = new InstanceUpdateRequest {Online = true, Id = this.Instance};
        await client.Instances.Update(updateRequest, token);
    }
}
