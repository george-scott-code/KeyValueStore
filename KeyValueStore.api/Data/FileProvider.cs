namespace KeyValueStore.api.Data;

public class FileProvider : IFileProvider
{
    private static string _dbPath = "D:\\source\\KeyValueStore\\Database\\Main";
    private static string _dbName = "db";
    
    //TODO: we will have to support one file for writing and potential multiple files for reading
    public string GetFilePath()
    {
        if(!Directory.Exists(_dbPath))
        {
            //create folder
            Directory.CreateDirectory(_dbPath);
        }
        var filePath = $"{_dbPath}/{_dbName}";
        if(!File.Exists(filePath))
        {
            using var _ = File.Create(filePath);
        }
        return filePath;
    }
}
