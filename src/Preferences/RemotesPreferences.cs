using Newtonsoft.Json;
using Tgstation.Server.CommandLineInterface.Services;

namespace Tgstation.Server.CommandLineInterface.Preferences;

[Preferences("remotes")]
public record struct RemotesPreferences
{
    public readonly Dictionary<string, TgsRemote> Remotes;
    public string? Current;

    public RemotesPreferences() : this(new Dictionary<string, TgsRemote>(), null)
    {
    }
    
    [JsonConstructor]
    public RemotesPreferences(Dictionary<string, TgsRemote> remotes, string? current)
    {
        Remotes = remotes;
        Current = current;
    }
}

public record struct TgsRemote(string Name, Uri Host);
