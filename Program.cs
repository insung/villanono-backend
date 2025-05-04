using System.Reflection;
using Microsoft.Extensions.Options;
using OpenSearch.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ElasticSearchSettingsModel>(
    builder.Configuration.GetSection("ElasticSearch")
);

var elasticSearchURL =
    Environment.GetEnvironmentVariable("ElasticSearch.URL") ?? "https://localhost:9200";

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<ElasticSearchSettingsModel>>().Value;

    var clientSettings = new ConnectionSettings(new Uri(elasticSearchURL))
        .BasicAuthentication(settings.UserName, settings.Password)
        .ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true)
        .EnableDebugMode()
        .DefaultIndex(settings.DefaultIndex);
    // .Authentication(new ApiKey(settings.ApiKey))

    return new OpenSearchClient(clientSettings);
});

builder.Services.AddScoped<IVillanonoRepository, VillanonoElasticSearchRepository>();
builder.Services.AddScoped<IIndexManagementService, IndexManagementService>();
builder.Services.AddScoped<ICSVReader, CSVReader>();
builder.Services.AddScoped<IRawDataService, RawDataService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IReportService, ReportService>();

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
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100_000_000; // 100MB로 증가
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// app.UseAuthorization();
app.MapControllers();
app.Run();
