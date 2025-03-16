public class ElasticSearchSettingsModel
{
    public string URL { get; set; } = "https://localhost:9200";
    public string ApiKey { get; set; }
    public string DefaultIndex { get; set; } = "villanono";
    public int BatchSize { get; set; } = 10000;
}
