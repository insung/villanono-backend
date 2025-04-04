using Microsoft.Extensions.Options;
using OpenSearch.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ElasticSearchSettingsModel>(
    builder.Configuration.GetSection("ElasticSearch")
);

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<ElasticSearchSettingsModel>>().Value;

    var clientSettings = new ConnectionSettings(new Uri(settings.URL))
        .BasicAuthentication(settings.UserName, settings.Password)
        .ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true)
        .EnableDebugMode()
        .DefaultIndex(settings.DefaultIndex);
    // .Authentication(new ApiKey(settings.ApiKey))

    return new OpenSearchClient(clientSettings);
});

builder.Services.AddScoped<IVillanonoRepository, VillanonoElasticSearchRepository>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IRepositoryService, RepositoryService>();
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.NumberHandling = System
            .Text
            .Json
            .Serialization
            .JsonNumberHandling
            .AllowNamedFloatingPointLiterals;
    });

// Swagger 서비스 추가
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// app.UseAuthorization();
app.MapControllers();
app.Run();
