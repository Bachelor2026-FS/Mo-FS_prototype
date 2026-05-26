namespace MoFs.Prototype.Models;

public class FoedselsnummerNormalizationResult
{
    public string? OriginalValue { get; init; }
    public string? NormalizedForFs { get; init; }
    public bool IsValid { get; init; }
    public List<string> ValidationMessages { get; init; } = new();
}
