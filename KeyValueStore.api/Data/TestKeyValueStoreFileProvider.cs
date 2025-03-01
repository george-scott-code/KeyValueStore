namespace KeyValueStore.api.Data;

public class TestFileProvider : IFileProvider
{
    private readonly string _dbPath = "D:\\source\\KeyValueStore\\db.txt";

    public TestFileProvider(string? dbPath = null)
    {
        if(dbPath != null)
        {
            _dbPath = dbPath;
        }
        else
        {
            FileStream fileStream = File.Open(_dbPath, FileMode.Open);

            fileStream.SetLength(0);
            fileStream.Close();
        }
    }

    public string GetFilePath()
    {
        return _dbPath;
    }
}
