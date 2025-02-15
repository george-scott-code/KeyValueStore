using KeyValueStore.api.Store;
namespace KeyValueStore;

public class IndexedTextStoreTests
{
    [Fact]
    public void WhenTheKeyHasBeenSet()
    {
        var store = new IndexedTextStore();

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);
    }

    [Fact]
    public void WhenTheKeyHasNotBeenSet()
    {
        var store = new IndexedTextStore();

        var result = store.Get("Goodbye");
        Assert.Null(result);
    }


    [Fact]
    public void WhenTheKeyHasBeenSetToAnEmptyValue()
    {
        var store = new IndexedTextStore();
        store.Set("foo", string.Empty);

        var result = store.Get("Goodbye");
        Assert.Equal(string.Empty, result);
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