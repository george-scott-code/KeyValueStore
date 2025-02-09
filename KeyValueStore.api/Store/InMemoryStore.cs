namespace KeyValueStore.api.Store;

public class InMemoryStore : IKeyValueStore
{
    private static readonly Dictionary<string, string> _store;

    static InMemoryStore()
    {
        _store = new Dictionary<string, string>();
    }

    public void Set(string key, string value)
    {
        _store[key] = value;
    }

    public string Get(string key)
    {
        _store.TryGetValue(key, out var value);
        return value is null ? string.Empty : value;
    }
}