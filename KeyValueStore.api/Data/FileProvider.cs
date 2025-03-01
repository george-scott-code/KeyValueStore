namespace KeyValueStore.api.Data;

public class FileProvider : IFileProvider
{
    private static string _dbPath = "D:\\source\\KeyValueStore\\Database\\Main";
    private static string _dbName = "db";
    
    //TODO: we will have to support one file for writing and potential multiple files for reading
    public string GetFilePath()
    {
        return $"{_dbPath}/{_dbName}.txt";
    }
}
