using KeyValueStore.api.Store;
namespace KeyValueStore;

public class KeyValueStoreTests
{

    // todo: database provider / configuration
    [Fact]
    public void Test1()
    {
        string dbPath = "D:\\source\\KeyValueStore\\db.txt";
        FileStream fileStream = File.Open(dbPath, FileMode.Open);

        fileStream.SetLength(0);
        fileStream.Close();

        var store = new InMemoryStore();

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);
    }

    [Fact]
    public void Test2()
    {
        string dbPath = "D:\\source\\KeyValueStore\\db.txt";
        FileStream fileStream = File.Open(dbPath, FileMode.Open);

        fileStream.SetLength(0);
        fileStream.Close();

        var store = new TextStore();

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);
    }

    [Fact]
    public void WhenTheSameKeyHasBeenSetTwice_ReturnTheLatestValue()
    {
        string dbPath = "D:\\source\\KeyValueStore\\db.txt";
        FileStream fileStream = File.Open(dbPath, FileMode.Open);

        fileStream.SetLength(0);
        fileStream.Close();

        var store = new TextStore();

        store.Set("Hello", "World");
        store.Set("Hello", "Dog");

        var result = store.Get("Hello");
        Assert.Equal("Dog", result);
    }

}