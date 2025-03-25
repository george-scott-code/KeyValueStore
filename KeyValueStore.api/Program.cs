using KeyValueStore.api.Data;
using KeyValueStore.api.Store;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IFileProvider, FileProvider>();
builder.Services.AddSingleton<IndexedTextStore, IndexedTextStore>();
builder.Services.AddSingleton<Configuration, Configuration>();

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.Configure(options =>
    {
        options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                            | ActivityTrackingOptions.TraceId
                                            | ActivityTrackingOptions.ParentId;
    }).AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
    });
});

var app = builder.Build();

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
