using System.Text.Json.Serialization;

namespace MoFs.Prototype.Models;

public class FsPayload
{
    [JsonPropertyName("input")]
    public List<FsInputRecord> Input { get; set; } = new();
}

public class FsInputRecord
{
    [JsonPropertyName("eierOrganisasjonskode")]
    public string EierOrganisasjonskode { get; set; } = string.Empty;

    [JsonPropertyName("student")]
    public FsStudent Student { get; set; } = new();

    [JsonPropertyName("studierett")]
    public FsStudierett Studierett { get; set; } = new();
}

public class FsStudent
{
    [JsonPropertyName("fodselsnummer")]
    public string Fodselsnummer { get; set; } = string.Empty;

    [JsonPropertyName("fornavn")]
    public string Fornavn { get; set; } = string.Empty;

    [JsonPropertyName("etternavn")]
    public string Etternavn { get; set; } = string.Empty;

    [JsonPropertyName("studentgrunnlagId")]
    public string StudentgrunnlagId { get; set; } = string.Empty;
}

public class FsStudierett
{
    [JsonPropertyName("studierettstatusId")]
    public string StudierettstatusId { get; set; } = string.Empty;

    [JsonPropertyName("kullId")]
    public string KullId { get; set; } = string.Empty;

    [JsonPropertyName("fraDato")]
    public string FraDato { get; set; } = string.Empty;

    [JsonPropertyName("fraTermin")]
    public FsTermin FraTermin { get; set; } = new();
}

public class FsTermin
{
    [JsonPropertyName("ar")]
    public int Ar { get; set; }

    [JsonPropertyName("betegnelse")]
    public string Betegnelse { get; set; } = string.Empty;
}
