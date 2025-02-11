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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        var store = new IndexedTextStore();

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