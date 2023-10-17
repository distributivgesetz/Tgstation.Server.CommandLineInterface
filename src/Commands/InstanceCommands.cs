namespace Tgstation.Server.CommandLineInterface.Commands;

using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("instances list"), UsedImplicitly]
public class ListInstancesCommand : BaseCommand
{
    private readonly IRemoteRegistry remotes;
    private readonly ISessionManager sessions;

    public ListInstancesCommand(IRemoteRegistry remotes, ISessionManager sessions)
    {
        this.remotes = remotes;
        this.sessions = sessions;
    }

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares) =>
        middlewares.UseMiddleware<EnsureCurrentSessionMiddleware>();

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var currentRemote = this.remotes.GetCurrentRemote();
    }
}
