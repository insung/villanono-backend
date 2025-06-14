using OpenSearch.Client;

public class DataRepository : IDataRepository
{
    readonly OpenSearchClient opensearchClient;

    public DataRepository(OpenSearchClient elasticsearchClient)
    {
        this.opensearchClient = elasticsearchClient;
    }

    public async Task BulkInsertData<T>(List<T> records, string indexName)
        where T : VillanonoBaseModel
    {
        var response = await opensearchClient.BulkAsync(b => b.Index(indexName).IndexMany(records));
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "BulkInsert failed"
        );
    }

    public async Task<IReadOnlyCollection<T>> GetData<T>(
        VillanonoDataType dataType,
        DateOnly beginDate,
        DateOnly endDate,
        string dong,
        string gu,
        string si = "서울특별시",
        string indexName = "villanono-*"
    )
        where T : VillanonoBaseModel
    {
        var searchRequest = new SearchRequest(indexName)
        {
            Query = new BoolQuery
            {
                Must = new QueryContainer[]
                {
                    new TermQuery { Field = Infer.Field<T>(f => f.DataType), Value = dataType },
                    new MatchQuery { Field = Infer.Field<T>(f => f.Dong), Query = dong },
                    new MatchQuery { Field = Infer.Field<T>(f => f.Gu), Query = gu },
                    new MatchQuery { Field = Infer.Field<T>(f => f.Si), Query = si },
                },
                Filter = new QueryContainer[]
                {
                    new DateRangeQuery
                    {
                        Field = Infer.Field<T>(f => f.ContractDate),
                        GreaterThanOrEqualTo = beginDate.ToString("yyyyMMdd"),
                        LessThanOrEqualTo = endDate.ToString("yyyyMMdd"),
                    },
                },
            },
        };

        var response = await opensearchClient.SearchAsync<T>(searchRequest);
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetData Failed"
        );
        return response?.Documents ?? Array.Empty<T>();
    }
}
