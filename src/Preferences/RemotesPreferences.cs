namespace Tgstation.Server.CommandLineInterface.Preferences;

using Newtonsoft.Json;
using Services;

[Preferences("remotes")]
public record struct RemotesPreferences
{
    public Dictionary<string, TgsRemote> Remotes { get; }
    public string? Current { get; set; }

    public RemotesPreferences() : this(new Dictionary<string, TgsRemote>(), null)
    {
    }

    [JsonConstructor]
    public RemotesPreferences(Dictionary<string, TgsRemote> remotes, string? current)
    {
        this.Remotes = remotes;
        this.Current = current;
    }
}

public record struct TgsRemote(string Name, Uri Host);
