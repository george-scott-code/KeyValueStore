using KeyValueStore.api.Data;
using KeyValueStore.api.Store;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IFileProvider, FileProvider>();
        builder.Services.AddSingleton<IndexedTextStore, IndexedTextStore>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        var indexedTextStore = app.Services.GetService<IndexedTextStore>() ?? throw new ArgumentNullException();
        
        app.MapGet("/value", (string key) =>
        {
            return indexedTextStore.Get(key);
        })
        .WithName("GetValue")
        .WithOpenApi();

        app.MapPost("/value", (string key, string value) =>
        {
            indexedTextStore.Set(key, value);
        })
        .WithName("PostValue")
        .WithOpenApi();

        app.MapDelete("/value", (string key) =>
        {
            indexedTextStore.Remove(key);
        })
        .WithName("Remove")
        .WithOpenApi();

        app.Run();
    }
}