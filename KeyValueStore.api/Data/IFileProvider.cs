namespace KeyValueStore.api.Data;

public interface IFileProvider
{
    public string[] GetReadFilePaths();
    public string GetWriteFilePath();
}
