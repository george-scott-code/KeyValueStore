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
            Directory.CreateDirectory(_dbPath);
            var filePath = $"{_dbPath}/{_dbName}_{DateTime.UtcNow:yyyyMMddTHHmmss}.db";
            using FileStream _ = File.Create(filePath);

            return filePath;  
        }

        var files = Directory.GetFiles(_dbPath, $"{_dbName}_*", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
        {
           var filePath = $"{_dbPath}/{_dbName}_{DateTime.UtcNow:yyyyMMddTHHmmss}.db" ;
           using FileStream _ = File.Create(filePath);

           return filePath;
        }

        // TODO: return the latest
        return files[0];
    }
}
