using KeyValueStore.lib.Store;
namespace KeyValueStore;

public class KeyValueStoreTests
{

    // todo: database provider / configuration
    [Fact]
    public void Test1()
    {
        var store = new InMemoryStore();

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);

        store.Remove("Hello");
        result = store.Get("Hello"); 
        Assert.Equal("", result);
    }
}