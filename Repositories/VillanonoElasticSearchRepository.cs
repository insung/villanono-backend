using System.Net;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;
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

    public async Task<IReadOnlyCollection<T>> GetData<T>(
        string indexName,
        int beginDate,
        int endDate,
        string dong,
        string gu,
        string si = "서울특별시"
    )
    {
        var searchRequest = new SearchRequest(indexName)
        {
            Query = new BoolQuery
            {
                Must = new List<Query>
                {
                    new TermQuery("dong") { Value = dong },
                    new TermQuery("gu") { Value = gu },
                    new TermQuery("si") { Value = si },
                    new DateRangeQuery("contractDate")
                    {
                        Gte = beginDate.ToString(),
                        Lte = endDate.ToString(),
                    },
                },
            },
        };

        var response = await elasticsearchClient.SearchAsync<T>(searchRequest);
        return response.Documents;
    }
}
