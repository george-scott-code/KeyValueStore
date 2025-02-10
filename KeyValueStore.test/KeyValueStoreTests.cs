using KeyValueStore.api.Store;
namespace KeyValueStore;

public class KeyValueStoreTests
{
    [Fact]
    public void Test1()
    {
        var store = new InMemoryStore();

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);
    }

    [Fact]
    public void Test2()
    {
        var store = new TextStore();

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);
    }

}