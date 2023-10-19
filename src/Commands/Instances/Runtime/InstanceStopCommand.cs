namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Runtime;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Middlewares;
using Middlewares.Implementations;
using Models;
using Services;
using Sessions;

[Command("instance stop")]
public class InstanceStopCommand : BaseSessionCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required long Id { get; init; }

    public InstanceStopCommand(ISessionManager sessions) : base(sessions)
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
        var updateRequest = new InstanceUpdateRequest {Online = false, Id = this.Id};
        await client.Instances.Update(updateRequest, token);
    }
}
