namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Runtime;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("instance stop")]
public class InstanceStopCommand : BaseCommand
{
    private readonly ISessionManager sessions;

    [CommandParameter(0)]
    public required long Id { get; init; }

    public InstanceStopCommand(ISessionManager sessions) => this.sessions = sessions;

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares)
    {
        middlewares.UseMiddleware<EnsureCurrentSessionMiddleware>();
        middlewares.UseMiddleware<UserUnauthorizedHandler>();
        middlewares.UseMiddleware<RequestFailHandlerMiddleware>();
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.sessions.ResumeSessionOrReprompt(console);
        var token = console.RegisterCancellationHandler();
        var updateRequest = new InstanceUpdateRequest {Online = false, Id = this.Id};
        await client.Instances.Update(updateRequest, token);
    }
}
