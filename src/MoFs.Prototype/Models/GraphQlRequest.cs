using System.Text.Json.Serialization;

namespace MoFs.Prototype.Models;

public class GraphQlRequest
{
    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;

    [JsonPropertyName("variables")]
    public object Variables { get; set; } = new();
}
