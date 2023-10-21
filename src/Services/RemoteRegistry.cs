namespace Tgstation.Server.CommandLineInterface.Services;

using Persistence;

public interface IRemoteRegistry
{
    IReadOnlyCollection<string> AvailableRemotes { get; }
    void AddRemote(string name, Uri uri);
    bool ContainsRemote(string name);
    TgsRemote GetRemote(string name);
    bool HasCurrentRemote();
    TgsRemote GetCurrentRemote();
    void SetCurrentRemote(string name);
    void SaveRemotes();
    void RemoveRemote(string name);
}

public sealed class RemoteRegistry : IRemoteRegistry
{
    private readonly IPersistenceManager preferences;
    private RemotesPreferences remotes;

    private TgsRemote? CurrentRemote =>
        this.remotes.Current != null ? this.remotes.Remotes[this.remotes.Current] : null;

    public IReadOnlyCollection<string> AvailableRemotes => this.remotes.Remotes.Keys;

    public RemoteRegistry(IPersistenceManager prefs)
    {
        this.preferences = prefs;
        this.remotes = this.preferences.ReadData<RemotesPreferences>();
    }

    public void AddRemote(string name, Uri uri) => this.remotes.Remotes.Add(name, new TgsRemote(name, uri));

    public bool ContainsRemote(string name) => this.remotes.Remotes.ContainsKey(name);

    public TgsRemote GetRemote(string name) => this.remotes.Remotes[name];

    public bool HasCurrentRemote() => this.CurrentRemote != null;

    public TgsRemote GetCurrentRemote() => this.CurrentRemote!.Value;

    public void SetCurrentRemote(string? name)
    {
        if (name != null)
        {
            if (this.ContainsRemote(name))
            {
                this.remotes.Current = name;
            }
            else
            {
                throw new ArgumentException($"Remote {name} not recognized", nameof(name));
            }
        }
        else
        {
            this.remotes.Current = null;
        }
    }

    public void SaveRemotes() => this.preferences.WriteData(this.remotes);

    public void RemoveRemote(string name)
    {
        if (!this.ContainsRemote(name))
        {
            throw new ArgumentException($"Remote {name} not recognized");
        }

        if (this.remotes.Current == name)
        {
            this.SetCurrentRemote(null);
        }

        this.remotes.Remotes.Remove(name);
    }
}
