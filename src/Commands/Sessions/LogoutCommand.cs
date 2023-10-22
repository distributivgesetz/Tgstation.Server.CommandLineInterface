namespace Tgstation.Server.CommandLineInterface.Commands.Sessions;

using CliFx.Attributes;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("logout", Description = "Drops the current session.")]
public sealed class LogoutCommand : BaseCommand
{
    private readonly IRemoteRegistry remotes;
    private readonly ISessionManager sessions;

    public LogoutCommand(ISessionManager sessions, IRemoteRegistry remotes)
    {
        this.sessions = sessions;
        this.remotes = remotes;
    }

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares) =>
        middlewares.UseMiddleware<EnsureCurrentRemoteMiddleware>();

    protected override ValueTask RunCommandAsync(ICommandContext context)
    {
        var currentRemote = this.remotes.GetCurrentRemote();

        if (!this.sessions.HasSession(currentRemote.Name))
        {
            return default;
        }

        this.sessions.DropSession(currentRemote.Name);
        this.sessions.SaveSessions();
        return default;
    }
}
