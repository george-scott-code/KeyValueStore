using KeyValueStore.api.Data;

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

        fs.Write(new byte[] { (byte)keyBytes.Length });
        fs.Write(keyBytes);
        // todo: consider max length
        fs.Write(new byte[] { (byte)valueBytes.Length });
        fs.Write(valueBytes);

        // todo: ensure file size does not exceed 2gb
        index[key] = new ByteData((int) offset + 2 + keyBytes.Length, valueBytes.Length);
    }

    public void BuildIndex()
    {
        using FileStream fs = new(_fileProvider.GetFilePath(), FileMode.Open, FileAccess.Read);
        var isIndexing = true;

        while(isIndexing)
        {
            var offset = fs.Position;
            var keyLength = fs.ReadByte();
            if(keyLength == -1)
            {
                isIndexing = false;
            }
            else
            {
                var keyBytes = new byte[keyLength];
                fs.ReadExactly(keyBytes, 0, keyLength);
                
                var key = System.Text.Encoding.UTF8.GetString(keyBytes);
                var valueLength = fs.ReadByte();
                var valueBytes = new byte[valueLength];
                fs.ReadExactly(valueBytes, 0, valueLength);
                
                index[key] = new ByteData((int) offset + 2 + keyBytes.Length, valueBytes.Length);
            }
        }
    }
}

public record ByteData(int Offset, int Length);
