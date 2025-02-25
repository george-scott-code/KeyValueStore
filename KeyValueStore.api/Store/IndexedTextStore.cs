namespace KeyValueStore.api.Store;

public class IndexedTextStore : IKeyValueStore
{
    private readonly Dictionary<string, ByteData> index = [];
    private readonly IKeyValueStoreFileProvider _fileProvider;

    public IndexedTextStore(IKeyValueStoreFileProvider fileProvider)
    {
        _fileProvider = fileProvider;

        using FileStream fs = new(_fileProvider.GetFilePath(), FileMode.Open, FileAccess.Read);

        if(fs.Length != 0)
        {
            BuildIndex();
        }
    }

    public string? Get(string key)
    {
        if(!index.TryGetValue(key, out ByteData? byteData))
        {
            return null;
        }

        using FileStream fs = new(_fileProvider.GetFilePath(), FileMode.Open, FileAccess.Read);
        fs.Seek(byteData.Offset, SeekOrigin.Begin);
        
        var byteBufffer = new byte[byteData.Length];
        fs.ReadExactly(byteBufffer, 0, byteData.Length);

        return System.Text.Encoding.UTF8.GetString(byteBufffer);
    }

    public void Set(string key, string value)
    {
        using FileStream fs = new(_fileProvider.GetFilePath(), FileMode.Append);
        fs.Seek(0, SeekOrigin.End);
        var offset = fs.Position;

        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] valueBytes = System.Text.Encoding.UTF8.GetBytes(value);

        // todo: we will have to consider writing the byte length to be able to rebuild the index
        fs.Write(keyBytes);
        fs.Write(valueBytes);

        // todo: ensure file size does not exceed 2gb
        index[key] = new ByteData((int)offset + keyBytes.Length, valueBytes.Length);
    }

    public void BuildIndex()
    {
        throw new NotImplementedException();
    }
}

public record ByteData(int Offset, int Length);

public interface IKeyValueStoreFileProvider
{
    public string GetFilePath();
}

public class KeyValueStoreFileProvider : IKeyValueStoreFileProvider
{
    private static string dbPath = "D:\\source\\KeyValueStore\\db.txt";
    public string GetFilePath()
    {
        return dbPath;
    }
}

public class TestKeyValueStoreFileProvider : IKeyValueStoreFileProvider
{
    private string _dbPath = "D:\\source\\KeyValueStore\\db.txt";

    public TestKeyValueStoreFileProvider(string? dbPath = null)
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
