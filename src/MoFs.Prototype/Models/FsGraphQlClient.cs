using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MoFs.Prototype.Models;

namespace MoFs.Prototype.Services;

public class FsGraphQlClient
{
    private readonly HttpClient _httpClient;
    private readonly FsGraphQlOptions _options;

    public FsGraphQlClient(HttpClient httpClient, FsGraphQlOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<string> OpprettStudenterAsync(FsPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        var mutation = """
            mutation Mutation($input: [OpprettStudenterInput!]!) {
              opprettStudenter(input: $input) {
                errors {
                  ... on UgyldigInput {
                    path
                    message
                  }
                  ... on UgyldigFodselsdatoFraFodselsnummerError {
                    path
                    message
                  }
                  ... on UgyldigFodselsnummer {
                    path
                    message
                  }
                  ... on UgyldigNorskTelefonnummer {
                    path
                    message
                  }
                  ... on StudentManglerGskOgStudentgrlError {
                    path
                    message
                  }
                  ... on OppretteStudierettPaUtgattStudieprogramError {
                    path
                    message
                  }
                  ... on OppretteStudierettPaLukketKullterminmError {
                    path
                    message
                  }
                  ... on OppretteStudierettPaLukketStartterminError {
                    path
                    message
                  }
                  ... on PrivatEpostErIkkeUnik {
                    path
                    message
                  }
                }
              }
            }
            """;

        var requestBody = new GraphQlRequest
        {
            Query = mutation,
            Variables = new
            {
                input = payload.Input
            }
        };

        var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue(_options.AuthorizationHeader, _options.ApiKey);
        }

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        Console.WriteLine("=== GRAPHQL REQUEST ===");
        Console.WriteLine(json);
        Console.WriteLine("=== END GRAPHQL REQUEST ===");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        Console.WriteLine("=== GRAPHQL RESPONSE ===");
        Console.WriteLine(responseContent);
        Console.WriteLine("=== END GRAPHQL RESPONSE ===");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"FS GraphQL kall feilet. Status: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {responseContent}");
        }

        return responseContent;
    }
}
