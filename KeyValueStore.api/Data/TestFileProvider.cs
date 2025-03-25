namespace KeyValueStore.api.Data;

public class TestFileProvider : IFileProvider
{
    private readonly string _dbName = "db.txt";
    private readonly string _dbPath = "D:\\source\\KeyValueStore\\Database\\Test";

    public TestFileProvider(Configuration? configuration = null)
    {
        if(configuration != null)
        {
            _dbPath = configuration.Path;
            _dbName = configuration.Name;
        }
        else
        {
            FileStream fileStream = File.Open($"{_dbPath}/{_dbName}", FileMode.Open);

            fileStream.SetLength(0);
            fileStream.Close();
        }
    }

    public string DbPath() => _dbPath;

    public string[] GetReadFilePaths()
    {
        return [$"{_dbPath}\\{_dbName}"];
    }

    public Segment GetWriteFilePath()
    {
        return new Segment(_dbPath, _dbName);
    }
}

public record Segment(string Path, string Name);