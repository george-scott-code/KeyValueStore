using KeyValueStore.api.Store;
namespace KeyValueStore;

public class IndexedTextStoreTests
{
    [Fact]
    public void WhenTheKeyHasBeenSet()
    {
        var store = new IndexedTextStore(new TestKeyValueStoreFileProvider());

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);
    }

    [Fact]
    public void WhenTheKeyHasNotBeenSet()
    {
        var store = new IndexedTextStore(new TestKeyValueStoreFileProvider());

        var result = store.Get("Goodbye");
        Assert.Null(result);
    }


    [Fact]
    public void WhenTheKeyHasBeenSetToAnEmptyValue()
    {
        var store = new IndexedTextStore(new TestKeyValueStoreFileProvider());

        store.Set("foo", string.Empty);

        var result = store.Get("foo");
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Indexed_WhenTheSameKeyHasBeenSetTwice_ReturnTheLatestValue()
    {
        var store = new IndexedTextStore(new TestKeyValueStoreFileProvider());

        store.Set("Hello", "World");
        store.Set("Hello", "Dog");

        var result = store.Get("Hello");
        Assert.Equal("Dog", result);
    }

    [Fact]
    public void WhenTheKeyHasBeenSetToAValueWithAComma()
    {
        var store = new IndexedTextStore(new TestKeyValueStoreFileProvider());
        store.Set("foo", "Good,bye");

        var result = store.Get("foo");
        Assert.Equal("Good,bye", result);
    }
}