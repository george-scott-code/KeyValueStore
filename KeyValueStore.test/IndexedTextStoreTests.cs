using KeyValueStore.api.Data;
using KeyValueStore.api.Store;
using Microsoft.Extensions.Logging.Abstractions;

namespace KeyValueStore.test;

public class IndexedTextStoreTests
{
    [Fact]
    public void WhenTheKeyHasBeenSet()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());
        store.Set("Hello", "World");

        var result = store.Get("Hello");
        Assert.Equal("World", result);
    }

    [Fact]
    public void WhenTheKeyHasNotBeenSet()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());

        var result = store.Get("Goodbye");
        Assert.Equal("", result);
    }

    [Fact]
    public void Set_WhenTheKeyIsEmpty()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());
        store.Set("", "world");

        var result = store.Get("");

        Assert.Equal("world", result);
    }
    
    [Fact]
    public void Set_WhenTheValueIsEmpty()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());
        store.Set("Hello", "");

        var result = store.Get("Hello");

        Assert.Equal("", result);
    }

    [Fact]
    public void WhenTheKeyHasBeenSetToAnEmptyValue()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());
        store.Set("foo", string.Empty);

        var result = store.Get("foo");
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Indexed_WhenTheSameKeyHasBeenSetTwice_ReturnTheLatestValue()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());
        store.Set("Hello", "World");
        store.Set("Hello", "Dog");

        var result = store.Get("Hello");
        Assert.Equal("Dog", result);
    }

    [Fact]
    public void WhenTheKeyHasBeenSetToAValueWithAComma()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());
        store.Set("foo", "Good,bye");

        var result = store.Get("foo");
        Assert.Equal("Good,bye", result);
    }

    [Fact]
    public void WhenTheKeyHasBeenDeleted()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());
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
        var store = new IndexedTextStore(
            new TestFileProvider("D:\\source\\KeyValueStore\\Database\\Test", "db_no_index"), new NullLogger<IndexedTextStore>());

        var result = store.Get("foo");
        Assert.Equal("bar", result);
        var result2 = store.Get("bar");
        Assert.Equal("foo", result2);
    }
    
    [Fact]
    public void WhenTheFileNeedsToBeReIndexedWithDeletedKey()
    {
        var store = new IndexedTextStore(
            new TestFileProvider("D:\\source\\KeyValueStore\\Database\\Test", "db_no_index_delete"), new NullLogger<IndexedTextStore>());

        var result = store.Get("foo");
        Assert.Equal("bar", result);
        var result2 = store.Get("bar");
        Assert.Equal("", result2);
    }
}