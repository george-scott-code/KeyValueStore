namespace KeyValueStore.api.Data;

public class FileProvider : IFileProvider
{
    private static string dbPath = "D:\\source\\KeyValueStore\\db.txt";
    public string GetFilePath()
    {
        return dbPath;
    }
}
