namespace KeyValueStore.api.Store;

public class IndexedTextStore : IKeyValueStore
{
    private static readonly Dictionary<string, long> index;

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
        if(!index.TryGetValue(key, out long offset))
        {
            return null;
        }

        string dbPath = "D:\\source\\KeyValueStore\\db.txt";

        using FileStream fs = new(dbPath, FileMode.Open, FileAccess.Read);
        fs.Seek(offset, SeekOrigin.Begin);
        using StreamReader sw = new StreamReader(fs);

        string? value = null;
        string? line = sw.ReadLine();

        // todo: null
        string[] parts = line.Split(',');
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
        var offset = fs.Seek(0, SeekOrigin.End);
        using StreamWriter sw = new(fs);

        // todo: write, format, csv
        sw.WriteLine($"{key},{value}");

        index[key] = offset;
    }
}