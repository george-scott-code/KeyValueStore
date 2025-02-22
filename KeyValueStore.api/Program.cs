using KeyValueStore.api.Store;
using Microsoft.Extensions.FileProviders;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IKeyValueStoreFileProvider, KeyValueStoreFileProvider>();
        builder.Services.AddSingleton<IndexedTextStore, IndexedTextStore>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        var store = app.Services.GetService<IndexedTextStore>();

        app.MapGet("/value", (string key) =>
        {
            return store.Get(key);
        })
        .WithName("GetValue")
        .WithOpenApi();

        app.MapPost("/value", (string key, string value) =>
        {
            store.Set(key, value);
        })
        .WithName("PostValue")
        .WithOpenApi();

        app.Run();
    }
}