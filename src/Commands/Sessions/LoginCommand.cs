namespace Tgstation.Server.CommandLineInterface.Commands.Sessions;

using CliFx.Attributes;
using CliFx.Infrastructure;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("login", Description = "Logs in with an account.")]
public sealed class LoginCommand : BaseCommand
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
