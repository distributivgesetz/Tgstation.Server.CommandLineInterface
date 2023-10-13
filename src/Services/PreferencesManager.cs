namespace Tgstation.Server.CommandLineInterface.Services;

using Newtonsoft.Json;

public interface IPreferencesManager
{
    T? ReadPrefs<T>() where T : new();
    void WritePrefs<T>(T prefs) where T : new();
    ValueTask<T?> ReadPrefsAsync<T>() where T : new();
    ValueTask WritePrefsAsync<T>(T prefs) where T : new();
}

public class PreferencesManager : IPreferencesManager
{
    private readonly Dictionary<Type, string> typeToPrefsFilename = new();

    private string GetFileName(Type t)
    {
        if (this.typeToPrefsFilename.TryGetValue(t, out var name))
        {
            return name;
        }

        if
            (t.GetCustomAttributes(typeof(PreferencesAttribute), false).FirstOrDefault() is not PreferencesAttribute
                 attr || attr.Name is null)
        {
            throw new ArgumentException("Preferences does not have a file descriptor");
        }

        if (this.typeToPrefsFilename.ContainsValue(attr.Name))
        {
            throw new InvalidOperationException($"Duplicate preferences file definition found, type is {t.FullName}");
        }

        this.typeToPrefsFilename.Add(t, attr.Name);

        return this.typeToPrefsFilename[t];
    }

    public T? ReadPrefs<T>() where T : new()
    {
        var filePath = this.GetFileName(typeof(T));

        return !File.Exists(filePath)
            ? new T()
            : JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
    }

    public void WritePrefs<T>(T prefs) where T : new()
    {
        var serialized = JsonConvert.SerializeObject(prefs);
        var filePath = this.GetFileName(typeof(T));
        File.WriteAllText(filePath, serialized);
    }

    public ValueTask<T?> ReadPrefsAsync<T>() where T : new() => ValueTask.FromResult(this.ReadPrefs<T>());

    public ValueTask WritePrefsAsync<T>(T prefs) where T : new()
    {
        this.WritePrefs(prefs);
        return ValueTask.CompletedTask;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class PreferencesAttribute : Attribute
{
    public string Name { get; }

    public PreferencesAttribute(string name) => this.Name = name;
}
