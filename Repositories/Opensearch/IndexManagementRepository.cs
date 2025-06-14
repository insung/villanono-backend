using Microsoft.Extensions.Options;
using OpenSearch.Client;
using OpenSearch.Net;

public class IndexManagementRepository : IIndexManagementRepository
{
    readonly OpenSearchClient opensearchClient;
    readonly string defaultIndex;
    readonly string defaultTemplateName;

    public IndexManagementRepository(
        OpenSearchClient elasticsearchClient,
        IOptions<OpensearchSettingsModel> elasticSearchSettings
    )
    {
        this.opensearchClient = elasticsearchClient;
        defaultIndex = elasticSearchSettings.Value.DefaultIndex;
        defaultTemplateName = $"{defaultIndex}_template";
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
        try
        {
            var getResponse = await this.GetDefaultIndexTemplate();

            throw new InvalidOperationException(
                $"Default index template '{defaultTemplateName}' already exists."
            );
        }
        catch (HttpRequestException httpEx)
        {
            // Template 이 없으면 404 에러 발생하여 이는 무시함
        }

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
            $"_index_template/{defaultTemplateName}",
            cancellationToken,
            PostData.Serializable(request)
        );

        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "CreateDefaultIndexTemplate failed"
        );
    }

    public async ValueTask<GetIndexTemplateResponse> GetDefaultIndexTemplate()
    {
        var response = await opensearchClient.Indices.GetTemplateAsync(defaultTemplateName);

        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetDefaultIndexTemplate failed"
        );

        return response!;
    }

    public async ValueTask<GetMappingResponse> GetIndexMapping(string indexName)
    {
        var request = new GetMappingRequest(indexName);
        var response = await opensearchClient.Indices.GetMappingAsync(request);

        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetIndexMapping failed"
        );

        return response!;
    }
}
