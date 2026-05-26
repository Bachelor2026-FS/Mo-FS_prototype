using System.Globalization;
using System.Text;
using MoFs.Prototype.Configuration;
using MoFs.Prototype.Contracts;
using MoFs.Prototype.Models;

namespace MoFs.Prototype.Services;

public class FsTransformationService : IFsTransformationService
{
    private readonly FsIntegrationOptions _options;
    private readonly FoedselsnummerService _foedselsnummerService;

    public FsTransformationService(FsIntegrationOptions options, FoedselsnummerService foedselsnummerService)
    {
        _options = options;
        _foedselsnummerService = foedselsnummerService;
    }

    public FsInputRecord Transform(MobilityOnlineStudent source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var fraTerminBetegnelse = TermMapper.MapFraTerminBetegnelse(source.Semester);
        var kullTerminBetegnelse = TermMapper.MapKullTerminBetegnelse(source.Semester);

        var terminAr = TermMapper.CapYearToFsRule(ResolveTermYear(source));
        var fraDato = ResolveFraDato(source, terminAr, kullTerminBetegnelse);

        var kullKode = ResolveKullKode(source.StudyProgram);

        var fsFoedselsnummer = _foedselsnummerService.ToFsFriendlyFoedselsnummer(source.BirthValue);

        var studentgrunnlagId = EncodeFsId(
            _options.StudentgrunnlagIdPrefix,
            _options.EierOrganisasjonskode,
    	    _options.StudentgrunnlagIdValue);

        var studierettstatusId = EncodeFsId(
            _options.StudierettstatusPrefix,
            _options.EierOrganisasjonskode,
            _options.StudierettstatusValue);

        var kullId = EncodeFsId(
            _options.KullIdPrefix,
            _options.EierOrganisasjonskode,
            kullKode,
            terminAr.ToString(CultureInfo.InvariantCulture),
            kullTerminBetegnelse);

        return new FsInputRecord
        {
            EierOrganisasjonskode = _options.EierOrganisasjonskode,
            Student = new FsStudent
            {
                Fodselsnummer = fsFoedselsnummer,
                Fornavn = source.FirstName?.Trim() ?? string.Empty,
                Etternavn = source.LastName?.Trim() ?? string.Empty,
                StudentgrunnlagId = studentgrunnlagId
            },
            Studierett = new FsStudierett
            {
                StudierettstatusId = studierettstatusId,
                KullId = kullId,
                FraDato = fraDato.ToString("yyyy-MM-dd"),
                FraTermin = new FsTermin
                {
                    Ar = terminAr,
                    Betegnelse = fraTerminBetegnelse
                }
            }
        };
    }

    private int ResolveTermYear(MobilityOnlineStudent source)
    {
        if (!string.IsNullOrWhiteSpace(source.StudyYear))
        {
            var parts = source.StudyYear.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0 && int.TryParse(parts[0], out var firstYear))
            {
                return firstYear;
            }
        }

        if (TryParseDate(source.FromDate, out var fromDate))
        {
            return fromDate.Year;
        }

        throw new ArgumentException(
            $"Kunne ikke finne terminår for student '{source.FirstName} {source.LastName}'.");
    }

    private DateTime ResolveFraDato(MobilityOnlineStudent source, int terminAr, string kullTerminBetegnelse)
    {
        if (TryParseDate(source.FromDate, out var fromDate))
        {
            return fromDate;
        }

        return kullTerminBetegnelse switch
        {
            "HØST" => new DateTime(terminAr, 8, 1),
            "VÅR" => new DateTime(terminAr, 1, 1),
            "SOM" => new DateTime(terminAr, 6, 1),
            _ => throw new ArgumentException($"Kunne ikke lage fraDato for '{source.FirstName} {source.LastName}'.")
        };
    }

    private string ResolveKullKode(string? studyProgram)
    {
        Console.WriteLine($"DEBUG StudyProgram = '{studyProgram}'");

        if (string.IsNullOrWhiteSpace(studyProgram))
        {
            return "UKJENT";
        }

        var normalized = studyProgram.Trim();

        if (_options.ProgramToKullKode.TryGetValue(normalized, out var mapped))
        {
            return mapped.ToUpperInvariant();
        }

        var value = normalized.ToLowerInvariant();

        if (value.Contains("bachelor"))
        {
            return "BACHELOR";
        }

        if (value.Contains("master"))
        {
            return "MASTER";
        }

        if (value.Contains("phd") || value.Contains("ph.d") || value.Contains("doktor"))
        {
            return "PHD";
        }

        return "UKJENT";
    }

    private static bool TryParseDate(string? value, out DateTime parsed)
    {
        parsed = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return DateTime.TryParseExact(
                   value.Trim(),
                   "dd.MM.yyyy",
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out parsed)
               || DateTime.TryParseExact(
                   value.Trim(),
                   "yyyy-MM-dd",
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out parsed);
    }

    private static string EncodeFsId(string prefix, params string[] values)
    {
        var raw = $"{prefix}:{string.Join(",", values)}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
    }
}
