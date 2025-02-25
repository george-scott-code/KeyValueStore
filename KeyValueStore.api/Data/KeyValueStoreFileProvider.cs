namespace KeyValueStore.api.Data;

public class KeyValueStoreFileProvider : IKeyValueStoreFileProvider
{
    private static string dbPath = "D:\\source\\KeyValueStore\\db.txt";
    public string GetFilePath()
    {
        return dbPath;
    }
}
