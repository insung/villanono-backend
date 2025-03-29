using System.Text;
using System.Text.Json;

public class AlternativeElasticsearchClient
{
    string BaseURL { get; set; }
    HttpClient httpClient { get; }

    public AlternativeElasticsearchClient(string url, string? apiKey)
    {
        BaseURL = url;
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                true,
        };
        httpClient = new HttpClient(handler);

        if (apiKey != null)
            httpClient.DefaultRequestHeaders.Add("Authorization", $"ApiKey {apiKey}");
    }

    public async Task<ESStatisticsSummaryResponse> GetStatisticsSummary(
        VillanonoDataType dataType,
        int beginDate,
        int endDate,
        string dong,
        string gu,
        string si = "서울특별시",
        string indexName = "villanono-*"
    )
    {
        var elasticQuery = new ESStatisticsSummaryRequest
        {
            DataType = dataType,
            BeginDate = beginDate,
            EndDate = endDate,
            Dong = dong,
            Gu = gu,
            Si = si,
            GroupByKey = "contractYearMonth",
            GroupByValue = "transactionAmount",
        };

        var json = JsonSerializer.Serialize(elasticQuery);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(BaseURL + $"/{indexName}/_search", content);
        var statisticsSummary = JsonSerializer.Deserialize<
            ESAggregations<ESStatisticsSummaryResponse>
        >(await response.Content.ReadAsStringAsync());
        return statisticsSummary!.Aggregations;
    }
}
