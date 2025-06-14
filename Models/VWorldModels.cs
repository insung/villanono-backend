using System.Text.Json.Serialization;

public class VWorldGeocodeResponse
{
    [JsonPropertyName("response")]
    public GeocodeResponsePayload? Response { get; set; }
}

public class GeocodeResponsePayload
{
    [JsonPropertyName("service")]
    public GeocodeServiceInfo? Service { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("input")]
    public GeocodeInput? Input { get; set; }

    [JsonPropertyName("refined")]
    public GeocodeRefined? Refined { get; set; }

    [JsonPropertyName("result")]
    public GeocodeResult? Result { get; set; }
}

public class GeocodeServiceInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("operation")]
    public string? Operation { get; set; }

    [JsonPropertyName("time")]
    public string? Time { get; set; }
}

public class GeocodeInput
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

public class GeocodeRefined
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("structure")]
    public GeocodeAddressStructure? Structure { get; set; }
}

public class GeocodeAddressStructure
{
    [JsonPropertyName("level0")]
    public string? Level0 { get; set; }

    [JsonPropertyName("level1")]
    public string? Level1 { get; set; }

    [JsonPropertyName("level2")]
    public string? Level2 { get; set; }

    [JsonPropertyName("level3")]
    public string? Level3 { get; set; }

    [JsonPropertyName("level4L")]
    public string? Level4L { get; set; }

    [JsonPropertyName("level4LC")]
    public string? Level4LC { get; set; }

    [JsonPropertyName("level4A")]
    public string? Level4A { get; set; }

    [JsonPropertyName("level4AC")]
    public string? Level4AC { get; set; }

    [JsonPropertyName("level5")]
    public string? Level5 { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}

public class GeocodeResult
{
    [JsonPropertyName("crs")]
    public string? Crs { get; set; }

    [JsonPropertyName("point")]
    public GeocodePoint? Point { get; set; }
}

// "point" 객체 (좌표)
public class GeocodePoint
{
    [JsonPropertyName("x")]
    public string? X { get; set; } // 경도 (Longitude)

    [JsonPropertyName("y")]
    public string? Y { get; set; } // 위도 (Latitude)
}
