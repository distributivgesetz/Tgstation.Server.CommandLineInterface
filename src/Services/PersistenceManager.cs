namespace Tgstation.Server.CommandLineInterface.Services;

using Newtonsoft.Json;

public interface IPersistenceManager
{
    T? ReadData<T>() where T : new();
    void WriteData<T>(T prefs) where T : new();
    ValueTask<T?> ReadDataAsync<T>() where T : new();
    ValueTask WriteDataAsync<T>(T prefs) where T : new();
}

public class PersistenceManager : IPersistenceManager
{
    private readonly Dictionary<Type, string> typeToPrefsFilename = new();

    private string GetFileName(Type t)
    {
        if (this.typeToPrefsFilename.TryGetValue(t, out var name))
        {
            return name;
        }

        if (t.GetCustomAttributes(typeof(DataLocationAttribute),
                    false)
                .FirstOrDefault() is not DataLocationAttribute attr ||
            attr.Name is null)
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

    public T? ReadData<T>() where T : new()
    {
        var filePath = this.GetFileName(typeof(T));

        return !File.Exists(filePath)
            ? new T()
            : JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
    }

    public void WriteData<T>(T prefs) where T : new()
    {
        var serialized = JsonConvert.SerializeObject(prefs);
        var filePath = this.GetFileName(typeof(T));
        File.WriteAllText(filePath, serialized);
    }

    public ValueTask<T?> ReadDataAsync<T>() where T : new() => ValueTask.FromResult(this.ReadData<T>());

    public ValueTask WriteDataAsync<T>(T prefs) where T : new()
    {
        this.WriteData(prefs);
        return ValueTask.CompletedTask;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class DataLocationAttribute : Attribute
{
    public string Name { get; }

    public DataLocationAttribute(string name) => this.Name = name;
}
