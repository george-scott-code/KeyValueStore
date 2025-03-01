namespace KeyValueStore.api.Data;

public class TestFileProvider : IFileProvider
{
    private readonly string _dbName = "db";
    private readonly string _dbPath = "D:\\source\\KeyValueStore\\Database\\Test";

    public TestFileProvider(string? dbPath = null, string? dbName = null)
    {
        if(dbPath != null && dbName != null)
        {
            _dbPath = dbPath;
            _dbName = dbName;
        }
        else
        {
            FileStream fileStream = File.Open($"{_dbPath}/{_dbName}.txt", FileMode.Open);

            fileStream.SetLength(0);
            fileStream.Close();
        }
    }

    public string GetFilePath()
    {
        // when do we check file size?
        return $"{_dbPath}/{_dbName}.txt";
    }
}
