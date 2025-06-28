using System.Diagnostics.CodeAnalysis;

/// <summary>
/// 시, 구, 동 정보를 포함한 모델
/// </summary>
public class RegionModel
{
    public required string Si { get; init; }
    public required string Gu { get; init; }
    public required string Dong { get; init; }

    public RegionModel() { }

    [SetsRequiredMembers]
    public RegionModel(string si, string gu, string dong)
    {
        Si = si;
        Gu = gu;
        Dong = dong;
    }
}
