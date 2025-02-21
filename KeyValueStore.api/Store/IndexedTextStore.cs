namespace KeyValueStore.api.Store;

public class IndexedTextStore : IKeyValueStore
{
    private static readonly Dictionary<string, ByteData> index;
    private static string dbPath = "D:\\source\\KeyValueStore\\db.txt";

    static IndexedTextStore()
    {
        FileStream fileStream = File.Open(dbPath, FileMode.Open);

        fileStream.SetLength(0);
        fileStream.Close();

        index = [];
    }

    public string? Get(string key)
    {
        if(!index.TryGetValue(key, out ByteData? byteData))
        {
            return null;
        }

        using FileStream fs = new(dbPath, FileMode.Open, FileAccess.Read);
        fs.Seek(byteData.Offset, SeekOrigin.Begin);
        
        var byteBufffer = new byte[byteData.Length];
        fs.ReadExactly(byteBufffer, 0, byteData.Length);

        return System.Text.Encoding.UTF8.GetString(byteBufffer);
    }

    public void Set(string key, string value)
    {
        using FileStream fs = new(dbPath, FileMode.Append);
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
}

public record ByteData(int Offset, int Length);
