using Microsoft.Extensions.Logging;

namespace KeyValueStore.api.Data;

public class FileProvider(ILogger<FileProvider> logger) : IFileProvider
{
    private static string _dbPath = "D:\\source\\KeyValueStore\\Database\\Main";
    private static string _dbName = "db";

    //TODO: we will have to support one file for writing and potential multiple files for reading
    public string GetFilePath()
    {
        if(!Directory.Exists(_dbPath))
        {
            Directory.CreateDirectory(_dbPath);
        }

        var files = Directory.GetFiles(_dbPath, $"{_dbName}_*", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
        {
            var filePath = $"{_dbPath}/{_dbName}_{DateTime.UtcNow:yyyyMMddTHHmmss}.db" ;
            logger.LogInformation($"Creating new database file. {filePath}");
            using FileStream _ = File.Create(filePath);

            return filePath;
        }

        // TODO: return the latest
        return files[0];
    }
}
