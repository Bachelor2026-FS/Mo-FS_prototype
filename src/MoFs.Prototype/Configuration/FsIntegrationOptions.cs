namespace MoFs.Prototype.Configuration;

public class FsIntegrationOptions
{
    public string EierOrganisasjonskode { get; set; } = "7777";

    public string StudentgrunnlagIdPrefix { get; set; } = "100";
    public string StudentgrunnlagIdValue { get; set; } = "DSP";

    public string StudierettstatusPrefix { get; set; } = "112";
    public string StudierettstatusValue { get; set; } = "ORDOPPTAK";

    public string KullIdPrefix { get; set; } = "58";

    public Dictionary<string, string> ProgramToKullKode { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Bachelor i skipsfart og logistikk"] = "BACHELOR",
        ["Bachelor i natur- og miljøforvaltning"] = "BACHELOR",
        ["Bachelor i økonomi og ledelse"] = "BACHELOR",
        ["Bachelor i sosiologi"] = "BACHELOR",
        ["Bachelor i visuell kommunikasjon"] = "BACHELOR",
        ["Bachelor i kunst og design"] = "BACHELOR",
        ["Bachelorstudium i tradisjonskunst; tre-, metall-, tekstilhåndverk"] = "BACHELOR",
        ["Master i Systems Engineering"] = "MASTER"
    };

    public Dictionary<string, string> StudyProgramCodeOverrides { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Bachelor i skipsfart og logistikk"] = "BACHELOR",
        ["Bachelor i natur- og miljøforvaltning"] = "BACHELOR",
        ["Bachelor i økonomi og ledelse"] = "BACHELOR",
        ["Bachelor i sosiologi"] = "BACHELOR",
        ["Bachelor i visuell kommunikasjon"] = "BACHELOR",
        ["Bachelor i kunst og design"] = "BACHELOR",
        ["Bachelorstudium i tradisjonskunst; tre-, metall-, tekstilhåndverk"] = "BACHELOR",
        ["Master i Systems Engineering"] = "MASTER"
    };

    public Dictionary<string, string> StudyProgramTypeKeywords { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["master"] = "MASTER",
        ["bachelor"] = "BACHELOR"
    };
}
