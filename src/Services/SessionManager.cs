namespace Tgstation.Server.CommandLineInterface.Services;

using System.Text;
using Client;
using CliFx.Infrastructure;
using Preferences;

public interface ISessionManager
{
    Task<IServerClient> TryResumeSession(CancellationToken cancellationToken = default);
    bool HasSession(string remoteKey);

    Task<IServerClient> LoginToSession(IConsole console, bool saveSession = true,
        CancellationToken cancellationToken = default);

    void DropSession(string key);
    void SaveSessions();
}

public class BadSessionException : Exception
{
    public BadSessionException(string? message, Exception? innerException = null) : base(message, innerException)
    {
    }
}

public class SessionManager : ISessionManager
{
    private readonly IRemoteRegistry remotes;
    private readonly ITgsClientManager clientFactory;
    private readonly IPersistenceManager prefs;

    private SessionPersistence Sessions { get; }

    public SessionManager(IRemoteRegistry remotes, ITgsClientManager clientFactory, IPersistenceManager prefs)
    {
        this.remotes = remotes;
        this.clientFactory = clientFactory;
        this.prefs = prefs;
        this.Sessions = prefs.ReadData<SessionPersistence>();
    }

    public async Task<IServerClient> TryResumeSession(CancellationToken cancellationToken = default)
    {
        var currentRemote = this.GetCurrentRemote();

        if (!this.Sessions.Sessions.TryGetValue(currentRemote.Name, out var authSession))
        {
            throw new BadSessionException("Session does not exist");
        }

        if (authSession.ExpiryDate < DateTime.Now)
        {
            throw new BadSessionException("Session has expired");
        }

        return await this.clientFactory.CreateSessionWithToken(currentRemote.Host, authSession.Token,
            authSession.ExpiryDate, cancellationToken);
    }

    public bool HasSession(string remoteKey) => this.Sessions.Sessions.ContainsKey(remoteKey);

    public async Task<IServerClient> LoginToSession(IConsole console, bool saveSession = true,
        CancellationToken cancellationToken = default)
    {
        var currentRemote = this.GetCurrentRemote();

        var username = ReadLoginInput(console, "Username: ", false);
        var password = ReadLoginInput(console, "Password: ", true);

        IServerClient client;

        try
        {
            client = await this.clientFactory.CreateSessionWithLogin(currentRemote.Host, username, password,
                cancellationToken);
        }
        catch (ApiException e)
        {
            throw new BadSessionException("Api error occurred", innerException: e);
        }

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

    private TgsRemote GetCurrentRemote() => this.remotes.CurrentRemote!.Value;

    private static string ReadLoginInput(IConsole console, string prompt, bool hidden)
    {
        console.Output.Write(prompt);
        var phrase = new StringBuilder();
        ConsoleKey key;
        do
        {
            var keyInfo = console.ReadKey(intercept: hidden);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && phrase.Length > 0)
            {
                if (hidden)
                {
                    console.Output.Write("\b \b");
                }

                phrase.Length--;
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                if (hidden)
                {
                    console.Output.Write("*");
                }

                phrase.Append(keyInfo.KeyChar);
            }
        } while (key != ConsoleKey.Enter);

        return phrase.ToString();
    }
}
