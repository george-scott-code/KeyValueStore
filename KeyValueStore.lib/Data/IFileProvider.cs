namespace KeyValueStore.lib.Data;

public interface IFileProvider
{
    public string[] GetReadFilePaths();
    public Segment GetWriteFilePath();
    public string DbPath();
    Segment GetCompactionFilePath();
}
