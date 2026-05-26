namespace MoFs.Prototype.Services;

public static class TermMapper
{
    public static string MapFraTerminBetegnelse(string? semesterValue)
    {
        if (string.IsNullOrWhiteSpace(semesterValue))
        {
            throw new ArgumentException("Semesterverdi mangler.");
        }

        var value = semesterValue.Trim().ToLowerInvariant();

        if (value.Contains("autumn") || value.Contains("fall") || value.Contains("høst") || value.Contains("host"))
        {
            return "HOST";
        }

        if (value.Contains("spring") || value.Contains("vår") || value.Contains("var"))
        {
            return "VAR";
        }

        if (value.Contains("summer") || value.Contains("sommer") || value.Contains("som"))
        {
            return "SOM";
        }

        throw new ArgumentException($"Ukjent semesterverdi: {semesterValue}");
    }

    public static string MapKullTerminBetegnelse(string? semesterValue)
    {
        if (string.IsNullOrWhiteSpace(semesterValue))
        {
            throw new ArgumentException("Semesterverdi mangler.");
        }

        var value = semesterValue.Trim().ToLowerInvariant();

        if (value.Contains("autumn") || value.Contains("fall") || value.Contains("høst") || value.Contains("host"))
        {
            return "HØST";
        }

        if (value.Contains("spring") || value.Contains("vår") || value.Contains("var"))
        {
            return "VÅR";
        }

        if (value.Contains("summer") || value.Contains("sommer") || value.Contains("som"))
        {
            return "SOM";
        }

        throw new ArgumentException($"Ukjent semesterverdi: {semesterValue}");
    }

    public static int CapYearToFsRule(int year)
    {
        return year > 2024 ? 2024 : year;
    }
}
