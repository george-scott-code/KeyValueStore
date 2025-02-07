namespace KeyValueStore.api.Store;

public static class Store
{
    private static Dictionary<string, string> _store;

    static Store()
    {
        _store = new Dictionary<string, string>();
    }

    public static void Set(string key, string value)
    {
        _store[key] = value;
    }

    public static string Get(string key)
    {
        return _store[key];
    }
}