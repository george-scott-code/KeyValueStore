using Microsoft.Extensions.Logging;

namespace KeyValueStore.api.Data;

public class FileProvider : IFileProvider
{
    private static string _dbPath = "D:\\source\\KeyValueStore\\Database\\Main";
    private static string _dbName = "db";

    ILogger<FileProvider> _logger;

    // todo: inject configuration
    public FileProvider(ILogger<FileProvider> logger, string? dbPath = null, string? dbName = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if(dbPath is not null)
        {
            _dbPath = dbPath;
        }
        if(dbName is not null)
        {
            _dbName = dbName;
        }
    }

    public string DbPath() => _dbPath;

    //TODO: we will have to support one file for writing and potential multiple files for reading
    public string[] GetReadFilePaths()
    {


        var files = Directory.GetFiles(_dbPath, $"{_dbName}_*", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
        {
            var filePath = $"{_dbPath}/{_dbName}_{DateTime.UtcNow:yyyyMMddTHHmmss}.db" ;
            _logger.LogInformation($"Creating new database file. {filePath}");
            using FileStream _ = File.Create(filePath);

            return [filePath];
        }

        return [.. files.OrderBy(static x => 
            DateTime.ParseExact(x.Split('_')[^1].Split('.')[0], "yyyyMMddTHHmmss", null))];
    }

    public Segment GetWriteFilePath()
    {
        if(!Directory.Exists(_dbPath))
        {
            Directory.CreateDirectory(_dbPath);
        }

        var files = Directory.GetFiles(_dbPath, $"{_dbName}_*", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
        {
            var name = "{_dbName}_{DateTime.UtcNow:yyyyMMddTHHmmss}.db";
            var filePath = $"{_dbPath}/{name}";
            _logger.LogInformation($"Creating new database file. {filePath}");
            using FileStream _ = File.Create(filePath);

            return new Segment(_dbPath, name);
        }

        // TODO: fix this, null etc
        var file = files.OrderByDescending(static x => 
            DateTime.ParseExact(x.Split('_')[^1].Split('.')[0], "yyyyMMddTHHmmss", null)).FirstOrDefault();
        
        int segmentIndex = file.LastIndexOf('\\');
        return new Segment(file.Substring(0, segmentIndex), file.Substring(segmentIndex +1));
    }
}
