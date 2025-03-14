using System.Buffers.Binary;
using KeyValueStore.api.Data;

namespace KeyValueStore.api.Store;

public class IndexedTextStore : IKeyValueStore
{
    private readonly Dictionary<string, ByteData> index = [];
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<IndexedTextStore> _logger;

    public IndexedTextStore(IFileProvider fileProvider, ILogger<IndexedTextStore> logger)
    {
        _fileProvider = fileProvider;
        _logger = logger;
        
        var filePath = _fileProvider.GetFilePath();
        using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);

        if (fs.Length != 0)
        {
            BuildIndex();
        }
    }

    public string Get(string key)
    {
        if (!index.TryGetValue(key, out ByteData? byteData))
        {
            return string.Empty;
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

        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] valueBytes = System.Text.Encoding.UTF8.GetBytes(value);

        byte[] keyLengthBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(keyLengthBytes, keyBytes.Length);
        fs.Write(keyLengthBytes);
        fs.Write(keyBytes);


        // todo: consider max length
        byte[] valueLengthBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(valueLengthBytes, valueBytes.Length);
        fs.Write(valueLengthBytes);
        
        var offset = fs.Position;
        fs.Write(valueBytes);

        // todo: ensure file size does not exceed 2gb
        index[key] = new ByteData((int) offset, valueBytes.Length);
    }

    public void Remove(string key)
    {
        index.TryGetValue(key, out ByteData? byteData);

        if (byteData is null) 
        {
            return;
        }

        using FileStream fs = new(_fileProvider.GetFilePath(), FileMode.Append);
        fs.Seek(0, SeekOrigin.End);
        
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] keyLengthBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(keyLengthBytes, keyBytes.Length);
        
        fs.Write(keyLengthBytes);
        fs.Write(keyBytes);

        byte[] valueLengthBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(valueLengthBytes, 0);
        fs.Write(valueLengthBytes);

        index.Remove(key);
    }

    public void BuildIndex()
    {
        _logger.LogInformation("Rebuilding Index");
        
        using FileStream fs = new(_fileProvider.GetFilePath(), FileMode.Open, FileAccess.Read);
        var isIndexing = true;

        while (isIndexing)
        {
            if (fs.Position == fs.Length)
            {
                isIndexing = false;
                return;
            }

            var offset = fs.Position;
            var keyLengthBytes = new byte[4];

            fs.ReadExactly(keyLengthBytes, 0, 4);
            var keyLength = BinaryPrimitives.ReadInt32BigEndian(keyLengthBytes);

            var keyBytes = new byte[keyLength];
            fs.ReadExactly(keyBytes, 0, keyLength);
            
            var key = System.Text.Encoding.UTF8.GetString(keyBytes);

            var valueLengthBytes = new byte[4];
            fs.ReadExactly(valueLengthBytes, 0, 4);
            var valueLength = BinaryPrimitives.ReadInt32BigEndian(valueLengthBytes);

            if (valueLength == 0) // KV removed
            {
                index.Remove(key);
            }
            else
            {
                var valueBytes = new byte[valueLength];
                fs.ReadExactly(valueBytes, 0, valueLength);
                
                index[key] = new ByteData((int) offset + 8 + keyBytes.Length, valueBytes.Length);
            }
        }
    }
}

public record ByteData(int Offset, int Length);
