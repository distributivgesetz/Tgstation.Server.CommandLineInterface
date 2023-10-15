namespace Tgstation.Server.CommandLineInterface.Commands;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Services;

[Command("login")]
public class LoginCommand : ICommand
{
    private readonly IRemoteRegistry remotes;
    private readonly ISessionManager sessions;

    public LoginCommand(IRemoteRegistry remotes, ISessionManager sessions)
    {
        this.remotes = remotes;
        this.sessions = sessions;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var remote = this.GetCurrentRemote(this.remotes);

        if (this.sessions.HasSession(remote.Name))
        {
            await console.Output.WriteLineAsync($"Using session for {remote.Name}.");
        }

        var client = await this.sessions.LoginToSession(console);
    }
}

[Command("logout")]
public class LogoutCommand : ICommand
{
    private readonly ISessionManager sessions;
    private readonly IRemoteRegistry remotes;

    public LogoutCommand(ISessionManager sessions, IRemoteRegistry remotes)
    {
        this.sessions = sessions;
        this.remotes = remotes;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var currentRemote = this.GetCurrentRemote(this.remotes);
        if (!this.sessions.HasSession(currentRemote.Name))
        {
            return default;
        }

        this.sessions.DropSession(currentRemote.Name);
        this.sessions.SaveSessions();
        return default;
    }
}
