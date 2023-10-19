namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Management;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("instance create")]
public class InstanceCreateCommand : BaseCommand
{
    private readonly ISessionManager sessions;

    [CommandParameter(0)]
    public required string Name { get; init; }

    [CommandParameter(1)]
    public required string Path { get; init; }

    public InstanceCreateCommand(ISessionManager sessions) => this.sessions = sessions;

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares)
    {
        middlewares.UseMiddleware<RequestFailHandlerMiddleware>();
        middlewares.UseMiddleware<EnsureCurrentSessionMiddleware>();
        middlewares.UseMiddleware<UserUnauthorizedHandler>();
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.sessions.ResumeSessionOrReprompt(console);
        var request = new InstanceCreateRequest
        {
            Name = this.Name,
            Path = this.Path
        };
        await client.Instances.CreateOrAttach(request, console.RegisterCancellationHandler());
    }
}
