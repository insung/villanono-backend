public class OpensearchSettingsModel
{
    public string URL { get; set; } = "https://localhost:9200";
    public string? ApiKey { get; set; } = null;
    public string? UserName { get; set; } = null;
    public string? Password { get; set; } = null;
    public string DefaultIndex { get; set; } = "villanono";
    public int BatchSize { get; set; } = 10000;
}
