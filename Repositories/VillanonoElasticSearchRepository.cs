using System.Net;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
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
        defaultIndex = elasticSearchSettings.Value.DefaultIndex;
    }

    public async ValueTask Ping()
    {
        var response = await elasticsearchClient.PingAsync();
        CheckResponseFailed(response, "Ping failed");
    }

    public async ValueTask CreateIndex(string indexName)
    {
        var response = await elasticsearchClient.Indices.CreateAsync(indexName);
        CheckResponseFailed(response, "Create Index failed");
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

    private void CheckResponseFailed(
        ElasticsearchResponse elasticsearchResponse,
        string failedMessage
    )
    {
        if (
            !tryGetElasticSearchApiResponseCode(
                elasticsearchResponse,
                out HttpStatusCode responseCode,
                out Exception? innerException
            )
        )
        {
            throw new HttpRequestException(failedMessage, innerException, responseCode);
        }
    }

    public async Task BulkInsert<T>(List<T> records, string? indexName = null)
    {
        if (string.IsNullOrWhiteSpace(indexName))
        {
            indexName = defaultIndex;
        }

        var bulkRequest = new BulkRequest(indexName)
        {
            Operations = records
                .Select(record => new BulkIndexOperation<T>(record))
                .Cast<IBulkOperation>()
                .ToList(),
        };

        var response = await elasticsearchClient.BulkAsync(bulkRequest);
        CheckResponseFailed(response, "BulkInsert failed");
    }

    public async ValueTask<bool> HasIndex(string indexName)
    {
        var response = await elasticsearchClient.Indices.ExistsAsync(indexName);
        return response.Exists;
    }

    public async ValueTask DeleteIndex(string indexName)
    {
        var response = await elasticsearchClient.Indices.DeleteAsync(indexName);
        CheckResponseFailed(response, "DeleteIndex failed");
    }
}
