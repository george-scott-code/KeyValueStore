using KeyValueStore.api.Data;
using KeyValueStore.api.Store;

namespace KeyValueStore.test;

public class IndexedTextStoreTests
{
    [Fact]
    public void WhenTheKeyHasBeenSet()
    {
        var store = new IndexedTextStore(new TestFileProvider());

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);
    }

    [Fact]
    public void WhenTheKeyHasNotBeenSet()
    {
        var store = new IndexedTextStore(new TestFileProvider());

        var result = store.Get("Goodbye");
        Assert.Equal("", result);
    }


    [Fact]
    public void WhenTheKeyHasBeenSetToAnEmptyValue()
    {
        var store = new IndexedTextStore(new TestFileProvider());

        store.Set("foo", string.Empty);

        var result = store.Get("foo");
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Indexed_WhenTheSameKeyHasBeenSetTwice_ReturnTheLatestValue()
    {
        var store = new IndexedTextStore(new TestFileProvider());

        store.Set("Hello", "World");
        store.Set("Hello", "Dog");

        var result = store.Get("Hello");
        Assert.Equal("Dog", result);
    }

    [Fact]
    public void WhenTheKeyHasBeenSetToAValueWithAComma()
    {
        var store = new IndexedTextStore(new TestFileProvider());
        store.Set("foo", "Good,bye");

        var result = store.Get("foo");
        Assert.Equal("Good,bye", result);
    }

    [Fact]
    public void WhenTheKeyHasBeenDeleted()
    {
        var store = new IndexedTextStore(new TestFileProvider());

        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);

        store.Remove("Hello");
        var result2 = store.Get("Hello");
        Assert.Equal("", result2);
    }

    [Fact]
    public void WhenTheFileNeedsToBeReIndexed()
    {
        var store = new IndexedTextStore(new TestFileProvider("D:\\source\\KeyValueStore\\Database\\Test", "db_noIndex"));

        var result = store.Get("foo");
        Assert.Equal("Good,bye", result);
        var result2 = store.Get("bar");
        Assert.Equal("Good,bye", result2);
    }
}