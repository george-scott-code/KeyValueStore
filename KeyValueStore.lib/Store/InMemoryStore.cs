using System.Collections.Generic;

namespace KeyValueStore.lib.Store;

public class InMemoryStore : IKeyValueStore
{
    private static readonly Dictionary<string, string> _store = [];

    public void Set(string key, string value)
    {
        _store[key] = value;
    }

    public string Get(string key)
    {
        _store.TryGetValue(key, out var value);
        return value is null ? string.Empty : value;
    }

    public void Remove(string key)
    {
        _store.Remove(key);
    }
}
