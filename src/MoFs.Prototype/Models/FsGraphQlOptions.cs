namespace MoFs.Prototype.Models;

public class FsGraphQlOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string AuthorizationHeader { get; set; } = "Bearer";
}
