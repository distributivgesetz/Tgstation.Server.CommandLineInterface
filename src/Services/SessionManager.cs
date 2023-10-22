namespace Tgstation.Server.CommandLineInterface.Services;

using System.Text;
using Client;
using CliFx.Infrastructure;
using Persistence;

public interface ISessionManager
{
    ValueTask<IServerClient?> TryResumeSession(CancellationToken token = default);
    ValueTask<IServerClient> ResumeSession(CancellationToken token = default);
    bool HasSession(string remoteKey);

    ValueTask<IServerClient> LoginToSession(
        IConsole console,
        bool saveSession = true,
        CancellationToken cancellationToken = default);

    void DropSession(string key);
    void SaveSessions();
}

public sealed class SessionManager : ISessionManager
{
    private readonly ITgsClientManager clientFactory;
    private readonly IPersistenceManager prefs;
    private readonly IRemoteRegistry remotes;

    private SessionPersistence Sessions { get; }

    public SessionManager(IRemoteRegistry remotes, ITgsClientManager clientFactory, IPersistenceManager prefs)
    {
        this.remotes = remotes;
        this.clientFactory = clientFactory;
        this.prefs = prefs;
        this.Sessions = prefs.ReadData<SessionPersistence>();
    }

    public async ValueTask<IServerClient?> TryResumeSession(CancellationToken token = default)
    {
        try
        {
            return await this.ResumeSession(token);
        }
        catch (BadLoginException)
        {
            return null;
        }
    }

    public async ValueTask<IServerClient> ResumeSession(CancellationToken token = default)
    {
        var currentRemote = this.remotes.GetCurrentRemote();

        if (!this.Sessions.Sessions.TryGetValue(currentRemote.Name, out var authSession))
        {
            throw new BadLoginException("No session available");
        }

        if (authSession.ExpiryDate < DateTime.Now)
        {
            throw new BadLoginException("Session has expired");
        }

        try
        {
            return await this.clientFactory.CreateSessionWithToken(currentRemote.Host, authSession.Token,
                authSession.ExpiryDate, token);
        }
        catch (ApiException)
        {
            throw new BadLoginException("Credentials are invalid");
        }
    }

    public bool HasSession(string remoteKey) => this.Sessions.Sessions.ContainsKey(remoteKey);

    public async ValueTask<IServerClient> LoginToSession(
        IConsole console,
        bool saveSession = true,
        CancellationToken cancellationToken = default)
    {
        var currentRemote = this.remotes.GetCurrentRemote();

        await console.Output.WriteAsync("Username: ");
        var username = (await console.Input.ReadLineAsync())!;
        if (username.Length == 0)
        {
            throw new BadLoginException("Please provide a username.");
        }

        await console.Output.WriteAsync("Password: ");
        var password = ReadHiddenInput(console);
        if (password.Length == 0)
        {
            throw new BadLoginException("Please provide a password.");
        }

        // This should throw UnauthorizedException, which is handled in UserUnauthorizedHandler
        var client =
            await this.clientFactory.CreateSessionWithLogin(currentRemote.Host, username, password, cancellationToken);

        if (!saveSession)
        {
            return client;
        }

        var res = client.Token;
        this.Sessions.Sessions.Add(currentRemote.Name, new AuthSession(currentRemote.Name, res.Bearer!, res.ExpiresAt));
        this.SaveSessions();

        return client;
    }

    public void DropSession(string key)
    {
        if (!this.remotes.ContainsRemote(key))
        {
            throw new ArgumentException("Remote does not exist");
        }

        this.Sessions.Sessions.Remove(key);
    }

    public void SaveSessions() => this.prefs.WriteData(this.Sessions);

    private static string ReadHiddenInput(IConsole console)
    {
        var phrase = new StringBuilder();
        ConsoleKey key;
        do
        {
            var keyInfo = console.ReadKey(true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && phrase.Length > 0)
            {
                console.Output.Write("\b \b");
                phrase.Length--;
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                console.Output.Write("*");
                phrase.Append(keyInfo.KeyChar);
            }
        } while (key != ConsoleKey.Enter);

        console.Output.Write(console.Output.NewLine);

        return phrase.ToString();
    }
}

public class BadLoginException : Exception
{
    public BadLoginException(string message, Exception? innerException = null) : base(message,
        innerException)
    {
    }
}
