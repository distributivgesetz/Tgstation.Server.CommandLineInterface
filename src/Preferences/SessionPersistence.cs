namespace Tgstation.Server.CommandLineInterface.Preferences;

using Newtonsoft.Json;
using Services;

[DataLocation("sessions")]
public record struct SessionPersistence
{
    public Dictionary<string, AuthSession> Sessions { get; }

    public SessionPersistence() : this(new Dictionary<string, AuthSession>())
    {
    }

    [JsonConstructor]
    public SessionPersistence(Dictionary<string, AuthSession> sessions) => this.Sessions = sessions;
}

public record struct AuthSession(string RemoteName, string Token, DateTimeOffset ExpiryDate);
