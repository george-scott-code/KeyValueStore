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
        // byte[] valueBytesLength = System.Text.Encoding.UTF8.GetBytes(value.Length); 

        // todo: we will have to consider writing the byte length to be able to rebuild the index
        fs.Write(keyBytes);
        // todo: consider max length
        fs.Write(new byte[] { (byte)keyBytes.Length });
        fs.Write(valueBytes);

        // todo: ensure file size does not exceed 2gb
        index[key] = new ByteData((int) offset + 1 + keyBytes.Length, valueBytes.Length);
    }

    public void BuildIndex()
    {
        throw new NotImplementedException();
    }
}

public record ByteData(int Offset, int Length);
