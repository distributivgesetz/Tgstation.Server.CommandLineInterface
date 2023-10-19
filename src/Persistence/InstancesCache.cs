namespace Tgstation.Server.CommandLineInterface.Persistence;

using Newtonsoft.Json;
using Services;

[DataLocation("instances")]
public record struct InstancesCache
{
    public Dictionary<string, RemoteKeyToInstance> Cache { get; }

    public InstancesCache() : this(new Dictionary<string, RemoteKeyToInstance>())
    {
    }

    [JsonConstructor]
    public InstancesCache(Dictionary<string, RemoteKeyToInstance> cache) => this.Cache = cache;
}

public readonly record struct RemoteKeyToInstance(string Key, Dictionary<string, InstanceCacheItem> Instances);


public readonly record struct InstanceCacheItem(string Name, long Id);
