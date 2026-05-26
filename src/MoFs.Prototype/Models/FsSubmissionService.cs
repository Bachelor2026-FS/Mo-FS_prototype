using MoFs.Prototype.Models;

namespace MoFs.Prototype.Services;

public class FsSubmissionService
{
    private readonly MobilityOnlineParser _parser;
    private readonly FsTransformationService _transformer;
    private readonly FsGraphQlClient _fsGraphQlClient;

    public FsSubmissionService(
        MobilityOnlineParser parser,
        FsTransformationService transformer,
        FsGraphQlClient fsGraphQlClient)
    {
        _parser = parser;
        _transformer = transformer;
        _fsGraphQlClient = fsGraphQlClient;
    }

    public async Task<string> SendRowsAsync(IEnumerable<MobilityOnlineRow> rows, CancellationToken cancellationToken = default)
    {
        var payload = new FsPayload();

        foreach (var row in rows)
        {
            var parsed = _parser.Parse(row);
            var transformed = _transformer.Transform(parsed);
            payload.Input.Add(transformed);
        }

        return await _fsGraphQlClient.OpprettStudenterAsync(payload, cancellationToken);
    }
}
