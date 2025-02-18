namespace KeyValueStore.api.Store;

public class IndexedTextStore : IKeyValueStore
{
    private static readonly Dictionary<string, ByteData> index;

    static IndexedTextStore()
    {
        string dbPath = "D:\\source\\KeyValueStore\\db.txt";
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

        string dbPath = "D:\\source\\KeyValueStore\\db.txt";

        using FileStream fs = new(dbPath, FileMode.Open, FileAccess.Read);
        fs.Seek(byteData.Offset, SeekOrigin.Begin);
        System.Console.WriteLine(fs.Position);
        var byteBufffer = new byte[byteData.Length];
        fs.ReadExactly(byteBufffer, 0, byteData.Length);

        var kvpString = System.Text.Encoding.UTF8.GetString(byteBufffer);

        // todo: null
        string[] parts = kvpString.Split(',');
        string value = string.Empty;
        if (parts[0] == key)
        {
            value = parts[1];
        }
        return value;
    }

    // todo: should the key be an int or guid? does it need to be specified or returned?
    public void Set(string key, string value)
    {
        // todo: sanitize string
        string dbPath = "D:\\source\\KeyValueStore\\db.txt";

        using FileStream fs = new(dbPath, FileMode.Append);
        fs.Seek(0, SeekOrigin.End);
        var offset = fs.Position;
        
        string kvp = $"{key},{value}";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(kvp);
        fs.Write(bytes);

        // todo: ensure file size does not exceed 2gb
        System.Console.WriteLine($"Offset: {offset} Bytes Length: {bytes.Length}");
        index[key] = new ByteData((int)offset, bytes.Length);
    }
}

public record ByteData(int Offset, int Length);
