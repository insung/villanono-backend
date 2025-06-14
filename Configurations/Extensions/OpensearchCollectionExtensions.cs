using Microsoft.Extensions.Options;
using OpenSearch.Client;

public static class OpensearchCollectionExtensions
{
    public static IServiceCollection AddOpensearchServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var elasticSearchURL =
            Environment.GetEnvironmentVariable("ElasticSearch.URL") ?? "https://localhost:9200";
        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<OpensearchSettingsModel>>().Value;

            var clientSettings = new ConnectionSettings(new Uri(elasticSearchURL))
                .BasicAuthentication(settings.UserName, settings.Password)
                .ServerCertificateValidationCallback(
                    (sender, certificate, chain, sslPolicyErrors) => true
                )
                .EnableDebugMode()
                .DefaultIndex(settings.DefaultIndex);
            // .Authentication(new ApiKey(settings.ApiKey))

            return new OpenSearchClient(clientSettings);
        });

        services.AddScoped<IVillanonoCsvReader, VillanonoCsvReader>();

        // Repositories Injection
        services.AddScoped<IIndexManagementRepository, IndexManagementRepository>();
        services.AddScoped<IDataRepository, DataRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        // Services Injection
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<ILocationService, LocationService>();

        return services;
    }
}
