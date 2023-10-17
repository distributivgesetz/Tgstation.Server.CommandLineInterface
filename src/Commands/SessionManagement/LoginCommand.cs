namespace Tgstation.Server.CommandLineInterface.Commands.SessionManagement;

using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("login"), UsedImplicitly]
public class LoginCommand : BaseCommand
{
    private readonly IRemoteRegistry remotes;
    private readonly ISessionManager sessions;

    public LoginCommand(IRemoteRegistry remotes, ISessionManager sessions)
    {
        this.remotes = remotes;
        this.sessions = sessions;
    }

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares) =>
        middlewares.UseMiddleware<EnsureCurrentSessionMiddleware>();

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var remote = this.remotes.GetCurrentRemote();

        if (this.sessions.HasSession(remote.Name))
        {
            await console.Output.WriteLineAsync($"Using session for {remote.Name}.");
        }

        await this.sessions.LoginToSession(console);
    }
}
