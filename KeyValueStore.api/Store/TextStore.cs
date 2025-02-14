namespace KeyValueStore.api.Store;

public class TextStore : IKeyValueStore
{

    static TextStore()
    {
        string dbPath = "D:\\source\\KeyValueStore\\db2.txt";
        FileStream fileStream = File.Open(dbPath, FileMode.Open);

        fileStream.SetLength(0);
        fileStream.Close();
    }

    public string? Get(string key)
    {
        string dbPath = "D:\\source\\KeyValueStore\\db2.txt";

        using FileStream fs = new(dbPath, FileMode.Open, FileAccess.Read);
        using StreamReader sw = new StreamReader(fs);

        string? value = null;
        string? line;

        // todo: parses whole file to match last occurance, consider reading the file in reverse when switching to a binary format
        while ((line = sw.ReadLine()) != null)
        {
            var parts = line.Split(',');
            if (parts[0] == key)
            {
                value = parts[1];
            }
        }
        return value;
    }

    // todo: should the key be an int or guid? does it need to be specified or returned?
    public void Set(string key, string value)
    {
        // todo: sanitize string
        string dbPath = "D:\\source\\KeyValueStore\\db2.txt";

        using FileStream fs = new(dbPath, FileMode.Append);
        using StreamWriter sw = new(fs);

        // todo: write, format, csv
        sw.WriteLine($"{key},{value}");
    }
}