using Microsoft.Extensions.Options;
using OpenSearch.Client;
using OpenSearch.Net;

public class IndexManagementRepository : IIndexManagementRepository
{
    readonly OpenSearchClient opensearchClient;
    readonly string defaultIndex;

    public IndexManagementRepository(
        OpenSearchClient elasticsearchClient,
        IOptions<OpensearchSettingsModel> elasticSearchSettings
    )
    {
        this.opensearchClient = elasticsearchClient;
        defaultIndex = elasticSearchSettings.Value.DefaultIndex;
    }

    public async ValueTask CreateDataIndex<T>(string indexName)
        where T : VillanonoBaseModel
    {
        var response = await opensearchClient.Indices.CreateAsync(
            indexName,
            c => c.Map<T>(m => m.Properties(p => p.Text(t => t.Name(n => n.AddressNumber))))
        );
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "Create DataIndex failed"
        );
    }

    public async ValueTask<string> CreateIndex(string indexName)
    {
        var response = await opensearchClient.Indices.CreateAsync(indexName);
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "Create Index failed"
        );
        return indexName;
    }

    public async ValueTask DeleteIndex(string indexName)
    {
        var response = await opensearchClient.Indices.DeleteAsync(indexName);
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "DeleteIndex failed"
        );
    }

    public async ValueTask<bool> HasIndex(string indexName)
    {
        var response = await opensearchClient.Indices.ExistsAsync(indexName);
        return response.Exists;
    }

    public async ValueTask Ping()
    {
        var response = await opensearchClient.PingAsync();
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "Ping failed"
        );
    }

    public async ValueTask ReIndex(string sourceIndexName, string targetIndexName)
    {
        var response = await opensearchClient.ReindexOnServerAsync(
            new ReindexOnServerRequest
            {
                Source = new ReindexSource { Index = sourceIndexName },
                Destination = new ReindexDestination { Index = targetIndexName },
            }
        );
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "ReIndex failed"
        );
    }

    public async ValueTask CreateDefaultIndexTemplate(CancellationToken cancellationToken = default)
    {
        var templateName = $"{defaultIndex}_template";

        var request = new
        {
            index_patterns = new[] { $"{defaultIndex}-*" },
            template = new
            {
                mappings = new
                {
                    properties = new Dictionary<string, object>
                    {
                        {
                            "addressNumber",
                            new
                            {
                                type = "text",
                                fields = new { keyword = new { type = "keyword" } },
                            }
                        },
                        {
                            "roadName",
                            new
                            {
                                type = "text",
                                fields = new { keyword = new { type = "keyword" } },
                            }
                        },
                        {
                            "si",
                            new
                            {
                                type = "text",
                                fields = new { keyword = new { type = "keyword" } },
                            }
                        },
                        {
                            "gu",
                            new
                            {
                                type = "text",
                                fields = new { keyword = new { type = "keyword" } },
                            }
                        },
                    },
                },
            },
        };

        var response = await opensearchClient.LowLevel.DoRequestAsync<StringResponse>(
            OpenSearch.Net.HttpMethod.PUT,
            $"_index_template/{templateName}",
            cancellationToken,
            PostData.Serializable(request)
        );

        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "CreateDefaultIndex failed"
        );
    }
}
