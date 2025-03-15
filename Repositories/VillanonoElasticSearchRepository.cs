using System.Net;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport.Products.Elasticsearch;
using Microsoft.Extensions.Options;

public sealed class VillanonoElasticSearchRepository : IVillanonoRepository
{
    readonly ElasticsearchClient elasticsearchClient;
    readonly string defaultIndex;

    public VillanonoElasticSearchRepository(
        ElasticsearchClient elasticsearchClient,
        IOptions<ElasticSearchSettingsModel> elasticSearchSettings
    )
    {
        this.elasticsearchClient = elasticsearchClient;
        this.defaultIndex = elasticSearchSettings.Value.DefaultIndex;
    }

    public async ValueTask Ping()
    {
        var response = await elasticsearchClient.PingAsync();

        if (
            !tryGetElasticSearchApiResponseCode(
                response,
                out HttpStatusCode responseCode,
                out Exception? innerException
            )
        )
        {
            throw new HttpRequestException("Ping failed", innerException, responseCode);
        }
    }

    public async ValueTask CreateDefaultDatabase()
    {
        var response = await elasticsearchClient.Indices.CreateAsync(this.defaultIndex);

        if (
            !tryGetElasticSearchApiResponseCode(
                response,
                out HttpStatusCode responseCode,
                out Exception? innerException
            )
        )
        {
            throw new HttpRequestException("Create Database failed", innerException, responseCode);
        }
    }

    private bool tryGetElasticSearchApiResponseCode(
        ElasticsearchResponse elasticsearchResponse,
        out HttpStatusCode responseCode,
        out Exception? innerException
    )
    {
        innerException = null;
        var apiCallResponseCode = elasticsearchResponse?.ApiCallDetails?.HttpStatusCode ?? 500;
        responseCode = (HttpStatusCode)apiCallResponseCode;
        if (responseCode != HttpStatusCode.OK)
        {
            innerException = new Exception(
                elasticsearchResponse?.ApiCallDetails?.DebugInformation ?? ""
            );
        }
        return responseCode == HttpStatusCode.OK;
    }
}
