using Tgstation.Server.CommandLineInterface.Preferences;

namespace Tgstation.Server.CommandLineInterface.Services;

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
    private readonly IPreferencesManager _preferences;
    private RemotesPreferences _remotes;
    public TgsRemote? CurrentRemote => _remotes.Current != null ? _remotes.Remotes[_remotes.Current] : null;
    public IReadOnlyCollection<string> AvailableRemotes => _remotes.Remotes.Keys;

    public RemoteRegistry(IPreferencesManager prefs)
    {
        _preferences = prefs;
        _remotes = _preferences.ReadPrefs<RemotesPreferences>();
    }
    
    public void AddRemote(string name, Uri uri) => _remotes.Remotes.Add(name, new TgsRemote(name, uri));

    public bool ContainsRemote(string name) => _remotes.Remotes.ContainsKey(name);

    public TgsRemote GetRemote(string name) => _remotes.Remotes[name];
    public void SetCurrentRemote(string name) => _remotes.Current = name;

    public void SaveRemotes() => _preferences.WritePrefs(_remotes);
}
