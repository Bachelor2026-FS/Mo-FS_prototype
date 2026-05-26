using System.Globalization;
using System.Text.RegularExpressions;

namespace MoFs.Prototype.Services;

public class FoedselsnummerService
{
    private static readonly int[] W1 = [3, 7, 6, 1, 8, 9, 4, 5, 2];
    private static readonly int[] W2 = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];

    public string ToFsFriendlyFoedselsnummer(string? moBirthValue)
    {
        if (string.IsNullOrWhiteSpace(moBirthValue))
        {
            throw new ArgumentException("Fødselsverdi mangler.");
        }

        var cleaned = Regex.Replace(moBirthValue.Trim(), @"\D", "");

        if (cleaned.Length == 11)
        {
            return ConvertElevenDigitValue(cleaned);
        }

        if (TryParseDate(moBirthValue, out var date))
        {
            return BuildSyntheticFromDate(date);
        }

        if (cleaned.Length == 8 && DateTime.TryParseExact(
                cleaned,
                "ddMMyyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var eightDigitDate))
        {
            return BuildSyntheticFromDate(eightDigitDate);
        }

        throw new ArgumentException($"Ugyldig fødselsverdi: {moBirthValue}");
    }

    private string ConvertElevenDigitValue(string fnr)
    {
        var day = int.Parse(fnr.Substring(0, 2), CultureInfo.InvariantCulture);
        var month = int.Parse(fnr.Substring(2, 2), CultureInfo.InvariantCulture);
        var yearTwoDigits = fnr.Substring(4, 2);

        // Hvis det ser ut som et vanlig fnr/D/H/FH-nummer, hent ut faktisk dato og lag FS-vennlig nummer.
        if (TryExtractDateFromFoedselsnummer(fnr, out var birthDate))
        {
            return BuildSyntheticFromDate(birthDate);
        }

        // Hvis datoen allerede er "fiktiv-lignende", normaliser til FS-format basert på det vi kan bruke.
        if (month > 12)
        {
            return RebuildSynthetic(day, yearTwoDigits);
        }

        throw new ArgumentException($"Kunne ikke tolke fødselsnummerverdi: {fnr}");
    }

    private string BuildSyntheticFromDate(DateTime birthDate)
    {
        var day = birthDate.Day.ToString("00", CultureInfo.InvariantCulture);

        // FS-flagg: ugyldig måned > 12. Her brukes 55 slik dere beskrev.
        const string syntheticMonth = "55";

        var yearTwoDigits = birthDate.ToString("yy", CultureInfo.InvariantCulture);

        for (var individ = 100; individ <= 999; individ++)
        {
            var baseDigits = $"{day}{syntheticMonth}{yearTwoDigits}{individ:000}";
            if (TryCalculateControlDigits(baseDigits, out var k1, out var k2))
            {
                return baseDigits + k1 + k2;
            }
        }

        throw new InvalidOperationException($"Fant ikke gyldig FS-fødselsnummer for dato {birthDate:yyyy-MM-dd}.");
    }

    private string RebuildSynthetic(int day, string yearTwoDigits)
    {
        var dayString = day.ToString("00", CultureInfo.InvariantCulture);
        const string syntheticMonth = "55";

        for (var individ = 100; individ <= 999; individ++)
        {
            var baseDigits = $"{dayString}{syntheticMonth}{yearTwoDigits}{individ:000}";
            if (TryCalculateControlDigits(baseDigits, out var k1, out var k2))
            {
                return baseDigits + k1 + k2;
            }
        }

        throw new InvalidOperationException($"Fant ikke gyldig FS-fødselsnummer for dag {dayString} og år {yearTwoDigits}.");
    }

    private static bool TryCalculateControlDigits(string nineDigits, out int k1, out int k2)
    {
        k1 = 0;
        k2 = 0;

        if (nineDigits.Length != 9 || !nineDigits.All(char.IsDigit))
        {
            return false;
        }

        var digits = nineDigits.Select(c => c - '0').ToArray();

        var sum1 = 0;
        for (var i = 0; i < 9; i++)
        {
            sum1 += digits[i] * W1[i];
        }

        var remainder1 = sum1 % 11;
        k1 = 11 - remainder1;
        if (k1 == 11) k1 = 0;
        if (k1 == 10) return false;

        var tenDigits = digits.Concat([k1]).ToArray();

        var sum2 = 0;
        for (var i = 0; i < 10; i++)
        {
            sum2 += tenDigits[i] * W2[i];
        }

        var remainder2 = sum2 % 11;
        k2 = 11 - remainder2;
        if (k2 == 11) k2 = 0;
        if (k2 == 10) return false;

        return true;
    }

    private static bool TryParseDate(string? value, out DateTime parsed)
    {
        parsed = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var formats = new[]
        {
            "dd.MM.yyyy",
            "yyyy-MM-dd",
            "ddMMyyyy"
        };

        return DateTime.TryParseExact(
            value.Trim(),
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out parsed);
    }

    private static bool TryExtractDateFromFoedselsnummer(string fnr, out DateTime birthDate)
    {
        birthDate = default;

        if (fnr.Length != 11 || !fnr.All(char.IsDigit))
        {
            return false;
        }

        var day = int.Parse(fnr.Substring(0, 2), CultureInfo.InvariantCulture);
        var month = int.Parse(fnr.Substring(2, 2), CultureInfo.InvariantCulture);
        var year = int.Parse(fnr.Substring(4, 2), CultureInfo.InvariantCulture);
        var individ = int.Parse(fnr.Substring(6, 3), CultureInfo.InvariantCulture);

        // D-nummer
        if (day > 40)
        {
            day -= 40;
        }

        // H-nummer
        if (month > 40)
        {
            month -= 40;
        }

        if (day is < 1 or > 31 || month is < 1 or > 12)
        {
            return false;
        }

        var fullYear = ResolveFullYear(year, individ);
        if (fullYear == null)
        {
            return false;
        }

        try
        {
            birthDate = new DateTime(fullYear.Value, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static int? ResolveFullYear(int yearTwoDigits, int individ)
    {
        if (individ is >= 000 and <= 499)
        {
            return 1900 + yearTwoDigits;
        }

        if (individ is >= 500 and <= 749 && yearTwoDigits is >= 54 and <= 99)
        {
            return 1800 + yearTwoDigits;
        }

        if (individ is >= 500 and <= 999 && yearTwoDigits is >= 00 and <= 39)
        {
            return 2000 + yearTwoDigits;
        }

        if (individ is >= 900 and <= 999 && yearTwoDigits is >= 40 and <= 99)
        {
            return 1900 + yearTwoDigits;
        }

        return null;
    }
}
