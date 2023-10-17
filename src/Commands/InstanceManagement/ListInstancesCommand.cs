namespace Tgstation.Server.CommandLineInterface.Commands.InstanceManagement;

using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("list-instances"), UsedImplicitly]
public class ListInstancesCommand : BaseCommand
{
    private readonly ISessionManager sessions;

    public ListInstancesCommand(ISessionManager sessions) => this.sessions = sessions;

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares)
    {
        middlewares.UseMiddleware<EnsureCurrentSessionMiddleware>();
        middlewares.UseMiddleware<RequestFailHandlerMiddleware>();
        middlewares.UseMiddleware<UserUnauthorizedHandler>();
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var client = await this.sessions.ResumeSessionOrReprompt(console);

        var token = console.RegisterCancellationHandler();

        // first fetch to get page count
        var instances = await client.Instances.List(null, token);

        foreach (var instance in instances)
        {
            await console.Output.WriteLineAsync(instance.Name);
        }
    }
}
