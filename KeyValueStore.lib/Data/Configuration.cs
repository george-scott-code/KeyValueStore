namespace KeyValueStore.lib.Data;

public record Configuration
{
    public string Path { get; init; } = "D:\\source\\KeyValueStore\\Database\\Main";
    public string Name { get; init; }= "db";
    public long MaximumSegmentSize {get; init; } = 10000;
}
