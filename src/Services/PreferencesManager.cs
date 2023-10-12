using Newtonsoft.Json;

namespace Tgstation.Server.CommandLineInterface.Services;

public interface IPreferencesManager
{
    ValueTask<T?> ReadPrefsAsync<T>();
    ValueTask WritePrefsAsync<T>(T prefs);
}

public class PreferencesManager : IPreferencesManager
{
    readonly Dictionary<Type, string> typeToPrefsFilename = new();

    string GetFileName(Type t)
    {
        if (!typeToPrefsFilename.ContainsKey(t))
        {
            if (t.GetCustomAttributes(typeof(PreferencesAttribute), false)
                    .FirstOrDefault() is not PreferencesAttribute attr ||
                attr.Name is null)
            {
                throw new ArgumentException("Preferences does not have a file descriptor");
            }
            typeToPrefsFilename.Add(t, attr.Name);
        }

        return typeToPrefsFilename[t];
    }
    
    public async ValueTask<T?> ReadPrefsAsync<T>()
    {
        var filePath = GetFileName(typeof(T));
        
        return !File.Exists(filePath)
            ? default
            : JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(filePath));
    }

    public async ValueTask WritePrefsAsync<T>(T prefs)
    {
        var serialized = JsonConvert.SerializeObject(prefs);
        var filePath = GetFileName(typeof(T));
        await File.WriteAllTextAsync(filePath, serialized);
    }
}

[AttributeUsage(AttributeTargets.Class)]
class PreferencesAttribute : Attribute
{
    public string Name { get; }
    
    public PreferencesAttribute(string name)
    {
        Name = name;
    }
}