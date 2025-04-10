using KeyValueStore.lib.Data;
using KeyValueStore.lib.Store;
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
    public void WhenTheKeyHasBeenSet_CalledMultipleTimes()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());

        for (int i = 0; i < 1000; i++)
        {
            store.Set($"Hello", $"World{i}");
            var result = store.Get($"Hello");
            Assert.Equal($"World{i}", result);
        }
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
    public void Delete_WhenTheKeyDoesNotExist()
    {
        var store = new IndexedTextStore(new TestFileProvider(), new NullLogger<IndexedTextStore>());
        store.Remove("Hello");
    }

    // Indexing
    [Fact]
    public void WhenTheFileNeedsToBeReIndexed()
    {
        var config = new Configuration()
        {
            Path ="D:\\source\\KeyValueStore\\Database\\Test", 
            Name = "db_no_index.txt"
        };

        var store = new IndexedTextStore(
            new TestFileProvider(config), new NullLogger<IndexedTextStore>());

        var result = store.Get("foo");
        Assert.Equal("bar", result);
        var result2 = store.Get("bar");
        Assert.Equal("foo", result2);
    }
    
    [Fact]
    public void WhenTheFileNeedsToBeReIndexedWithDeletedKey()
    {
        var config = new Configuration()
        {
            Path = "D:\\source\\KeyValueStore\\Database\\Test", 
            Name = "db_no_index_delete.txt"
        };
        
        var store = new IndexedTextStore(
            new TestFileProvider(config), new NullLogger<IndexedTextStore>());

        var result = store.Get("foo");
        Assert.Equal("bar", result);
        var result2 = store.Get("bar");
        Assert.Equal("", result2);
    }

    // Segmentation

    [Fact]
    public void ReIndexedTheStore_WhenThereAreTwoSegments()
    {
        var config = new Configuration()
        {
            Path = "D:\\source\\KeyValueStore\\Database\\TestSegments",
            Name = "db"
        };

        var store = new IndexedTextStore(
            new FileProvider(new NullLogger<FileProvider>(), config), new NullLogger<IndexedTextStore>());
        
        var result = store.Get("segment");
        Assert.Equal("two", result);

        var result2 = store.Get("hello");
        Assert.Equal("world", result2);
    }
    
    [Fact]
    public void ReIndexedTheStore_WhenTheSegmentsAreCompacted()
    {
        var config = new Configuration()
        {
            Path = "D:\\source\\KeyValueStore\\Database\\TestSegmentCompaction",
            Name = "db"
        };

        var provider = new FileProvider(new NullLogger<FileProvider>(), config);
        var store = new IndexedTextStore(provider, new NullLogger<IndexedTextStore>());
            
        // Should be called by a process or scenario
        store.CompactSegments();
        
        var paths = provider.GetReadFilePaths();
        Assert.Equal(3, paths.Length);

        var result = store.Get("segment");
        Assert.Equal("two", result);

        var result2 = store.Get("hello");
        Assert.Equal("world", result2);

        // cleanup the compacted file
        // store.CleanupCompactedFiles();
        // TODO: before we can run this ^ we need file generation for the tests, so we can delete them
        paths = provider.GetReadFilePaths();
        //Assert.Equal(2, paths.Length);
        File.Delete(paths[0]);
    }

    [Fact]
    public void AddingKVP_WhenTheCurrentSegmentIsFull()
    {
        var config = new Configuration()
        {
            Path = "D:\\source\\KeyValueStore\\Database\\Segmentation",
            Name = "db",
            MaximumSegmentSize = 1000
        };

        var provider = new FileProvider(new NullLogger<FileProvider>(), config);
        var store = new IndexedTextStore(provider, new NullLogger<IndexedTextStore>()); 
        
        var paths = provider.GetReadFilePaths();

        Assert.Single(paths);

        var result = store.Get("Test file segmentation");
        Assert.Equal("Test file segmentation", result);

        store.Set("Test file segmentation", "Test file added");

        paths = provider.GetReadFilePaths();
        Assert.Equal(2, paths.Length);

        result = store.Get("Test file segmentation");
        Assert.Equal("Test file added", result);

        // cleanup new file
        var newFile = paths[1];
        File.Delete(newFile);
    }
}