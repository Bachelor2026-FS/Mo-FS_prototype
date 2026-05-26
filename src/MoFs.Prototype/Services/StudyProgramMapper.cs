using MoFs.Prototype.Configuration;

namespace MoFs.Prototype.Services;

public class StudyProgramMapper
{
    private readonly FsIntegrationOptions _options;

    public StudyProgramMapper(FsIntegrationOptions options)
    {
        _options = options;
    }

    public string ResolveProgramTypeCode(string? studyProgramName)
    {
        if (string.IsNullOrWhiteSpace(studyProgramName))
        {
            return "UKJENT";
        }

        if (_options.StudyProgramCodeOverrides.TryGetValue(studyProgramName, out var overrideCode))
        {
            return overrideCode;
        }

        foreach (var keyword in _options.StudyProgramTypeKeywords)
        {
            if (studyProgramName.Contains(keyword.Key, StringComparison.OrdinalIgnoreCase))
            {
                return keyword.Value;
            }
        }

        return Sanitize(studyProgramName);
    }

    private static string Sanitize(string value)
    {
        return value
            .Trim()
            .ToUpperInvariant()
            .Replace(" ", "_")
            .Replace("-", "_");
    }
}
