using System.Net;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

public sealed class VillanonoElasticSearchRepository : IVillanonoRepository
{
    readonly ElasticsearchClient elasticsearchClient;
    readonly string defaultDatabase;

    public VillanonoElasticSearchRepository(
        in string url,
        in string username,
        in string secret,
        in string defaultDatabase = "villanono"
    )
    {
        var clientSettings = new ElasticsearchClientSettings(new Uri(url))
            .Authentication(new BasicAuthentication(username, secret))
            .DefaultIndex(defaultDatabase);

        elasticsearchClient = new ElasticsearchClient(clientSettings);
        this.defaultDatabase = defaultDatabase;
    }

    public async ValueTask Ping()
    {
        var response = await elasticsearchClient.PingAsync();
    }

    public async ValueTask CreateDefaultDatabase()
    {
        var response = await elasticsearchClient.Indices.CreateAsync(defaultDatabase);
    }
}
