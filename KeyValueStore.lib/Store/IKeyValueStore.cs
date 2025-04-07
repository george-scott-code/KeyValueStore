namespace KeyValueStore.lib.Store;

public interface IKeyValueStore
{
    void Set(string key, string value);
    string? Get(string key);
    void Remove(string key);
}