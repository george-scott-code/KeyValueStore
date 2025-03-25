namespace KeyValueStore.api.Data;

public class FileProvider : IFileProvider
{
    private ILogger<FileProvider> _logger;
    private Configuration _config;

    public FileProvider(ILogger<FileProvider> logger, Configuration config, string? dbPath = null, string? dbName = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public string DbPath() => _config.Path;

    public string[] GetReadFilePaths()
    {
        var files = Directory.GetFiles(_config.Path, $"{_config.Name}_*", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
        {
            var filePath = $"{_config.Path}/{_config.Name}_{DateTime.UtcNow:yyyyMMddTHHmmss}.db" ;
            _logger.LogInformation($"Creating new database file. {filePath}");
            using FileStream _ = File.Create(filePath);

            return [filePath];
        }

        return [.. files.OrderBy(static x => 
            DateTime.ParseExact(x.Split('_')[^1].Split('.')[0], "yyyyMMddTHHmmss", null))];
    }

    public Segment GetWriteFilePath()
    {
        if(!Directory.Exists(_config.Path))
        {
            Directory.CreateDirectory(_config.Path);
        }

        var files = Directory.GetFiles(_config.Path, $"{_config.Name}_*", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
        {
            return CreateNewSegment();
        }

        // TODO: fix this, null etc, can we use dir info and only consider correct format?
        var file = files.OrderByDescending(static x => 
            DateTime.ParseExact(x.Split('_')[^1].Split('.')[0], "yyyyMMddTHHmmss", null)).FirstOrDefault();
        
        var fi = new FileInfo(file);
        if(fi.Length > _config.MaximumSegmentSize)
        {
            return CreateNewSegment();
        }

        int segmentIndex = file.LastIndexOf('\\');
        return new Segment(file[..segmentIndex], file.Substring(segmentIndex +1));
    }

    private Segment CreateNewSegment()
    {
        var name = $"{_config.Name}_{DateTime.UtcNow:yyyyMMddTHHmmss}.db";
        var filePath = $"{_config.Path}/{name}";
        _logger.LogInformation($"Creating new database segment. {filePath}");
        using FileStream _ = File.Create(filePath);

        return new Segment(_config.Path, name);
    }
}
