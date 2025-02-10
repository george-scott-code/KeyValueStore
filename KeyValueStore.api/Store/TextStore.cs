using Microsoft.VisualBasic;

namespace KeyValueStore.api.Store;

public class TextStore : IKeyValueStore
{

    static TextStore()
    {
    }

    public string? Get(string key)
    {
        string dbPath = "D:\\source\\KeyValueStore\\db.txt";

        using FileStream fs = new(dbPath, FileMode.Open, FileAccess.Read);
        using StreamReader sw = new StreamReader(fs);

        string? line;
        while ((line = sw.ReadLine()) != null)
        {
            var parts = line.Split(" ");
            if (parts[0] == key)
            {
                return parts[1];
            }
        }
        return null;
    }

    public void Set(string key, string value)
    {
        string dbPath = "D:\\source\\KeyValueStore\\db.txt";
        FileStream fileStream = File.Open(dbPath, FileMode.Open);

        fileStream.SetLength(0);
        fileStream.Close();


        using FileStream fs = new(dbPath, FileMode.Append);
        using StreamWriter sw = new(fs);

        sw.WriteLine($"{key} {value}");
    }
}