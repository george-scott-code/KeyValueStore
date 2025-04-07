using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KeyValueStore.lib.Data;
using Microsoft.Extensions.Logging;

namespace KeyValueStore.lib.Store;

public class IndexedTextStore : IKeyValueStore
{
    private ConcurrentDictionary<string, ByteData> index = [];
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<IndexedTextStore> _logger;

    public IndexedTextStore(IFileProvider fileProvider, ILogger<IndexedTextStore> logger)
    {
        _fileProvider = fileProvider;
        _logger = logger;
        
        var filePaths = _fileProvider.GetReadFilePaths();
        
        foreach (var filePath in filePaths)
        {
            BuildIndex(filePath);
        }        
    }

    public string Get(string key)
    {
        if (!index.TryGetValue(key, out ByteData? byteData))
        {
            return string.Empty;
        }

        var segmentPath = $"{_fileProvider.DbPath()}/{byteData.Segment}";
        using FileStream fs = new(segmentPath, FileMode.Open, FileAccess.Read);
        fs.Seek(byteData.Offset, SeekOrigin.Begin);
        
        var byteBufffer = new byte[byteData.Length];
        fs.ReadExactly(byteBufffer, 0, byteData.Length);

        return System.Text.Encoding.UTF8.GetString(byteBufffer);
    }

    public void Set(string key, string value)
    {
        Segment segment = _fileProvider.GetWriteFilePath();
        var filePath = $"{segment.Path}/{segment.Name}";
        
        using FileStream fs = new(filePath, FileMode.Append);
        fs.Seek(0, SeekOrigin.End);

        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] valueBytes = System.Text.Encoding.UTF8.GetBytes(value);

        byte[] keyLengthBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(keyLengthBytes, keyBytes.Length);
        fs.Write(keyLengthBytes);
        fs.Write(keyBytes);

        // todo: consider max length
        byte[] valueLengthBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(valueLengthBytes, valueBytes.Length);
        fs.Write(valueLengthBytes);
        
        var offset = fs.Position;
        try
        {
            fs.Write(valueBytes);
        }
        catch(Exception ex){
            _logger.LogError(ex, "Error when writing KVP");
        }

        // can we verify?

        index[key] = new ByteData((int) offset, valueBytes.Length, segment.Name);
    }

    public void Remove(string key)
    {
        index.TryGetValue(key, out ByteData? byteData);

        if (byteData is null) 
        {
            return;
        }
        Segment segment = _fileProvider.GetWriteFilePath();

        using FileStream fs = new($"{segment.Path}/{segment.Name}", FileMode.Append);
        fs.Seek(0, SeekOrigin.End);
        
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] keyLengthBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(keyLengthBytes, keyBytes.Length);
        
        fs.Write(keyLengthBytes);
        fs.Write(keyBytes);

        byte[] valueLengthBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(valueLengthBytes, 0);
        fs.Write(valueLengthBytes);

        index.Remove(key, out _);
    }

    public void BuildIndex(string filePath)
    {
        _logger.LogInformation("Rebuilding Index");
        
        using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
        var isIndexing = true;

        while (isIndexing)
        {
            if (fs.Position == fs.Length)
            {
                isIndexing = false;
                return;
            }

            var offset = fs.Position;
            var keyLengthBytes = new byte[4];

            fs.ReadExactly(keyLengthBytes, 0, 4);
            var keyLength = BinaryPrimitives.ReadInt32BigEndian(keyLengthBytes);

            var keyBytes = new byte[keyLength];
            fs.ReadExactly(keyBytes, 0, keyLength);
            
            var key = System.Text.Encoding.UTF8.GetString(keyBytes);

            var valueLengthBytes = new byte[4];
            fs.ReadExactly(valueLengthBytes, 0, 4);
            var valueLength = BinaryPrimitives.ReadInt32BigEndian(valueLengthBytes);

            if (valueLength == 0) // KV removed
            {
                index.Remove(key, out _);
            }
            else
            {
                var valueBytes = new byte[valueLength];
                fs.ReadExactly(valueBytes, 0, valueLength);
                
                var segmentName = filePath[(filePath.LastIndexOf('\\') + 1)..];
                index[key] = new ByteData((int) offset + 8 + keyBytes.Length, valueBytes.Length, segmentName);
            }
        }
    }

    // TODO: make sure this does not conflict with any get/set requests in progress
    public void CompactSegments()
    {
        Segment writeFile = _fileProvider.GetWriteFilePath(); // new writes
        Segment compactionFile = _fileProvider.GetCompactionFilePath(); // writes for compacted semgents (there may have been more writes...) 
        
        // TODO: its possible we will need to create multiple compaction files...
        var compactedIndex = new ConcurrentDictionary<string, ByteData>(); 

        foreach(var kvp in index)
        {
            if(kvp.Value.Segment == writeFile.Name)
            {
                compactedIndex[kvp.Key] = kvp.Value;
            }
            else
            {
                // if its not in the write file we can write it to a new file
                // then discard the segments that are nor writable, then switch the index
                _logger.LogInformation(message: $"Compacting File {kvp.Value.Segment}");

                var filePath = $"{compactionFile.Path}/{compactionFile.Name}";
                
                using FileStream fs = new(filePath, FileMode.Append);
                fs.Seek(0, SeekOrigin.End);

                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(kvp.Key);

                // TODO: we can shorcut some of the code in get, as we already have the index...
                var value = Get(kvp.Key);
                byte[] valueBytes = System.Text.Encoding.UTF8.GetBytes(value);

                byte[] keyLengthBytes = new byte[4];
                BinaryPrimitives.WriteInt32BigEndian(keyLengthBytes, keyBytes.Length);
                fs.Write(keyLengthBytes);
                fs.Write(keyBytes);

                // todo: consider max length
                byte[] valueLengthBytes = new byte[4];
                BinaryPrimitives.WriteInt32BigEndian(valueLengthBytes, valueBytes.Length);
                fs.Write(valueLengthBytes);
                
                var offset = fs.Position;
                try
                {
                    fs.Write(valueBytes);
                }
                catch(Exception ex){
                    _logger.LogError(ex, "Error when writing KVP");
                }
                compactedIndex[kvp.Key] = new ByteData((int) offset, valueBytes.Length, compactionFile.Name);
            }
        }
        // we need to update the index in one shot when all files are compacted
        index = compactedIndex;
    }
    
    // delete unreferenced files, this could happen independently
    public void CleanupCompactedFiles()
    {
        string[] readFiles = _fileProvider.GetReadFilePaths();

        var indexedFiles = index.Values.Select(x => $"{_fileProvider.DbPath()}\\{x.Segment}").ToArray();

        var unreferencedFiles = readFiles.Where(x => !indexedFiles.Contains(x)).ToArray();

        foreach (string file in unreferencedFiles)
        {
            File.Delete(file);
        }
    }
}
    

public record ByteData(int Offset, int Length, string Segment);