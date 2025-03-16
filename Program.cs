using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ElasticSearchSettingsModel>(
    builder.Configuration.GetSection("ElasticSearch")
);

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<ElasticSearchSettingsModel>>().Value;
    var clientSettings = new ElasticsearchClientSettings(new Uri(settings.URL))
        .Authentication(new ApiKey(settings.ApiKey))
        .ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true)
        .DefaultIndex(settings.DefaultIndex);

    return new ElasticsearchClient(clientSettings);
});

builder.Services.AddScoped<IVillanonoRepository, VillanonoElasticSearchRepository>();
builder.Services.AddScoped<IVillanonoLoadService, VillanonoDataService>();
builder.Services.AddControllers();

// Swagger 서비스 추가
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// app.UseAuthorization();
app.MapControllers();
app.Run();
