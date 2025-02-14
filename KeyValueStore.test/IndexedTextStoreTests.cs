using KeyValueStore.api.Store;
namespace KeyValueStore;

public class IndexedTextStoreTests
{
    [Fact]
    public void Test3()
    {
        var store = new IndexedTextStore();

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);
    }

    [Fact]
    public void Indexed_WhenTheSameKeyHasBeenSetTwice_ReturnTheLatestValue()
    {
        var store = new IndexedTextStore();

        store.Set("Hello", "World");
        store.Set("Hello", "Dog");

        var result = store.Get("Hello");
        Assert.Equal("Dog", result);
    }
}