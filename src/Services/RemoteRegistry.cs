namespace Tgstation.Server.CommandLineInterface.Services;

using Preferences;

public interface IRemoteRegistry
{
    TgsRemote? CurrentRemote { get; }
    IReadOnlyCollection<string> AvailableRemotes { get; }
    void AddRemote(string name, Uri uri);
    bool ContainsRemote(string name);
    TgsRemote GetRemote(string name);
    void SetCurrentRemote(string name);
    void SaveRemotes();
}

public class RemoteRegistry : IRemoteRegistry
{
    private readonly IPersistenceManager preferences;
    private RemotesPreferences remotes;

    public TgsRemote? CurrentRemote => this.remotes.Current != null ? this.remotes.Remotes[this.remotes.Current] : null;

    public IReadOnlyCollection<string> AvailableRemotes => this.remotes.Remotes.Keys;

    public RemoteRegistry(IPersistenceManager prefs)
    {
        this.preferences = prefs;
        this.remotes = this.preferences.ReadData<RemotesPreferences>();
    }

    public void AddRemote(string name, Uri uri) => this.remotes.Remotes.Add(name, new TgsRemote(name, uri));

    public bool ContainsRemote(string name) => this.remotes.Remotes.ContainsKey(name);

    public TgsRemote GetRemote(string name) => this.remotes.Remotes[name];

    public void SetCurrentRemote(string name) => this.remotes.Current = name;

    public void SaveRemotes() => this.preferences.WriteData(this.remotes);
}
