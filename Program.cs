var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var client = new VillanonoElasticSearchRepository(
    "http://localhost:9200",
    "elastic",
    "jcAFJWzwNtOaVSR+DZRi"
);

await client.Ping();
await client.CreateDefaultDatabase();

app.Run();
