using System.Reflection;
using Microsoft.Extensions.Options;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Opensearch 설정
builder.Services.Configure<OpensearchSettingsModel>(builder.Configuration.GetSection("Opensearch"));

builder.Services.AddOpensearchServices(builder.Configuration);

// Refit HttpClient 설정
builder.Services.Configure<VWorldSettingsModel>(builder.Configuration.GetSection("VWorld"));

builder
    .Services.AddRefitClient<IVWorldRepository>()
    .ConfigureHttpClient(
        (serviceProvider, client) =>
        {
            var vWorldSettings = serviceProvider
                .GetRequiredService<IOptions<VWorldSettingsModel>>()
                .Value;
            client.BaseAddress = new Uri(vWorldSettings.BaseURL);
        }
    );

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
