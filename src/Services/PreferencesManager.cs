using Newtonsoft.Json;

namespace Tgstation.Server.CommandLineInterface.Services;

public interface IPreferencesManager
{
    T? ReadPrefs<T>() where T : new();
    void WritePrefs<T>(T prefs) where T : new();
    ValueTask<T?> ReadPrefsAsync<T>() where T : new();
    ValueTask WritePrefsAsync<T>(T prefs) where T : new();
}

public class PreferencesManager : IPreferencesManager
{
    readonly Dictionary<Type, string> _typeToPrefsFilename = new();

    private string GetFileName(Type t)
    {
        if (_typeToPrefsFilename.TryGetValue(t, out var name))
        {
            return name;
        }

        if
            (t.GetCustomAttributes(typeof(PreferencesAttribute), false).FirstOrDefault() is not PreferencesAttribute
                 attr || attr.Name is null)
        {
            throw new ArgumentException("Preferences does not have a file descriptor");
        }

        if (_typeToPrefsFilename.ContainsValue(attr.Name))
        {
            throw new InvalidOperationException($"Duplicate preferences file definition found, type is {t.FullName}");
        }

        _typeToPrefsFilename.Add(t, attr.Name);

        return _typeToPrefsFilename[t];
    }

    public T? ReadPrefs<T>() where T : new()
    {
        var filePath = GetFileName(typeof(T));

        return !File.Exists(filePath)
            ? new T()
            : JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
    }

    public void WritePrefs<T>(T prefs) where T : new()
    {
        var serialized = JsonConvert.SerializeObject(prefs);
        var filePath = GetFileName(typeof(T));
        File.WriteAllText(filePath, serialized);
    }

    public ValueTask<T?> ReadPrefsAsync<T>() where T : new()
    {
        return ValueTask.FromResult(ReadPrefs<T>());
    }

    public ValueTask WritePrefsAsync<T>(T prefs) where T : new()
    {
        WritePrefs(prefs);
        return ValueTask.CompletedTask;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class PreferencesAttribute : Attribute
{
    public string Name { get; }

    public PreferencesAttribute(string name)
    {
        Name = name;
    }
}