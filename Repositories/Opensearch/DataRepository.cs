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

    public async Task<IReadOnlyCollection<T>> SearchBySiGuDong<T>(
        HashSet<VillanonoDataType> dataTypes,
        string si = "서울특별시",
        string? gu = null,
        string? dong = null,
        DateOnly? beginContractDate = null,
        DateOnly? endContractDate = null,
        double? beginTransactionAmount = null,
        double? endTransactionAmount = null,
        int? beginConstructYear = null,
        int? endConstructYear = null,
        double? beginExclusiveArea = null,
        double? endExclusiveArea = null,
        string indexName = "villanono-*"
    )
        where T : VillanonoBaseModel
    {
        var filterQueries = new List<QueryContainer>();

        var siTermQuery = new TermQuery();
        siTermQuery.Field = "si.keyword";
        siTermQuery.Value = si;
        filterQueries.Add(siTermQuery);

        if (!string.IsNullOrWhiteSpace(gu))
        {
            var guTermQuery = new TermQuery();
            guTermQuery.Field = "gu.keyword";
            guTermQuery.Value = gu;
            filterQueries.Add(guTermQuery);
        }

        if (!string.IsNullOrWhiteSpace(dong))
        {
            var dongTermQuery = new TermQuery();
            dongTermQuery.Field = "dong.keyword";
            dongTermQuery.Value = dong;
            filterQueries.Add(dongTermQuery);
        }

        // TermsQuery를 객체 초기화 구문으로 생성 시 컴파일러 오류가 발생할 수 있어,
        // 객체를 먼저 생성하고 속성을 순차적으로 할당하는 방식으로 수정합니다.
        var termsQuery = new TermsQuery();
        termsQuery.Field = "dataType";
        // Enum 값을 int로 캐스팅하고, TermsQuery가 요구하는 object 리스트로 변환
        termsQuery.Terms = dataTypes.Select(dt => (int)dt).Cast<object>().ToList();
        filterQueries.Add(termsQuery);

        // ContractDate Range (YYYYMMDD int format) - Using NumericRangeQuery
        if (beginContractDate.HasValue || endContractDate.HasValue)
        {
            var dateRangeQuery = new NumericRangeQuery();
            dateRangeQuery.Field = "contractDate";

            if (beginContractDate.HasValue)
            {
                // DateOnly를 YYYYMMDD int 형식으로 변환
                dateRangeQuery.GreaterThanOrEqualTo =
                    beginContractDate.Value.Year * 10000
                    + beginContractDate.Value.Month * 100
                    + beginContractDate.Value.Day;
            }
            if (endContractDate.HasValue)
            {
                // DateOnly를 YYYYMMDD int 형식으로 변환
                dateRangeQuery.LessThanOrEqualTo =
                    endContractDate.Value.Year * 10000
                    + endContractDate.Value.Month * 100
                    + endContractDate.Value.Day;
            }
            filterQueries.Add(dateRangeQuery);
        }

        // ConstructYear Range
        if (beginConstructYear.HasValue || endConstructYear.HasValue)
        {
            var constructYearRangeQuery = new NumericRangeQuery();
            constructYearRangeQuery.Field = "constructionYear";

            if (beginConstructYear.HasValue)
            {
                constructYearRangeQuery.GreaterThanOrEqualTo = beginConstructYear.Value;
            }
            if (endConstructYear.HasValue)
            {
                constructYearRangeQuery.LessThanOrEqualTo = endConstructYear.Value;
            }
            filterQueries.Add(constructYearRangeQuery);
        }

        // ExclusiveArea Range
        if (beginExclusiveArea.HasValue || endExclusiveArea.HasValue)
        {
            var exclusiveAreaRangeQuery = new NumericRangeQuery();
            exclusiveAreaRangeQuery.Field = "exclusiveArea";

            if (beginExclusiveArea.HasValue)
            {
                exclusiveAreaRangeQuery.GreaterThanOrEqualTo = beginExclusiveArea.Value;
            }
            if (endExclusiveArea.HasValue)
            {
                exclusiveAreaRangeQuery.LessThanOrEqualTo = endExclusiveArea.Value;
            }
            filterQueries.Add(exclusiveAreaRangeQuery);
        }

        // TransactionAmount Range
        if (beginTransactionAmount.HasValue || endTransactionAmount.HasValue)
        {
            var transactionAmountRangeQuery = new NumericRangeQuery();
            transactionAmountRangeQuery.Field = "transactionAmount";

            if (beginTransactionAmount.HasValue)
            {
                transactionAmountRangeQuery.GreaterThanOrEqualTo = beginTransactionAmount.Value;
            }
            if (endTransactionAmount.HasValue)
            {
                transactionAmountRangeQuery.LessThanOrEqualTo = endTransactionAmount.Value;
            }
            filterQueries.Add(transactionAmountRangeQuery);
        }

        var searchRequest = new SearchRequest(indexName)
        {
            Query = new BoolQuery { Filter = filterQueries.Any() ? filterQueries.ToArray() : null },
        };

        var response = await opensearchClient.SearchAsync<T>(searchRequest);
        OpensearchResponseHandler.CheckResponseFailed(
            response?.ApiCall?.HttpStatusCode,
            response?.ApiCall?.DebugInformation,
            "GetData Failed"
        );
        return response?.Documents ?? Array.Empty<T>();
    }

    public async Task<IReadOnlyCollection<T>> SearchByRoadName<T>(
        string roadName,
        string buildingName,
        int? contractDate,
        string indexName = "villanono-*"
    )
        where T : VillanonoBaseModel
    {
        var filterQueries = new List<QueryContainer>();

        if (contractDate.HasValue) // Check if nullable int has a value
        {
            var contractDateRangeQuery = new NumericRangeQuery();
            contractDateRangeQuery.Field = "contractDate";
            contractDateRangeQuery.GreaterThanOrEqualTo = contractDate.Value;
            filterQueries.Add(contractDateRangeQuery);
        }

        var roadNameMatchQuery = new MatchQuery();
        roadNameMatchQuery.Field = "roadName";
        roadNameMatchQuery.Query = roadName;

        var buildingNameMatchQuery = new MatchQuery();
        buildingNameMatchQuery.Field = "buildingName";
        buildingNameMatchQuery.Query = buildingName;

        var searchRequest = new SearchRequest(indexName)
        {
            Query = new BoolQuery
            {
                Must = new QueryContainer[] { roadNameMatchQuery, buildingNameMatchQuery },
                Filter = filterQueries.Any() ? filterQueries.ToArray() : null,
            },
        };

        var searchResponse = await opensearchClient.SearchAsync<T>(searchRequest);

        OpensearchResponseHandler.CheckResponseFailed(
            searchResponse?.ApiCall?.HttpStatusCode,
            searchResponse?.ApiCall?.DebugInformation,
            "SearchByRoadName Failed"
        );

        if (searchResponse?.Documents.Any() == true)
        {
            return searchResponse.Documents.ToList();
        }

        return Array.Empty<T>();
    }
}
