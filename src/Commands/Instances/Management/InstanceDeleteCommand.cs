namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Management;

using CliFx.Attributes;
using CliFx.Infrastructure;
using Middlewares;
using Middlewares.Implementations;
using Models;
using Services;
using Sessions;

[Command("instance delete")]
public class InstanceDeleteCommand : BaseSessionCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

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
