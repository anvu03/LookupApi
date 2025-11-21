namespace Api.Lookups;

// Service Layer
public class LookupOrchestrator
{
    private readonly GenericDapperLookupHandler _handler;
    
    // Minimal APIs resolve scoped services perfectly fine
    public LookupOrchestrator(GenericDapperLookupHandler handler)
    {
        _handler = handler;
    }

    public async Task<LookupResponse> ProcessBatchAsync(List<LookupRequest> requests, CancellationToken ct)
    {
        // We use Parallel.ForEach or Task.WhenAll here for performance
        var tasks = requests.Select(async req => 
        {
            var data = await _handler.FetchAsync(req, ct);
            return new { Key = req.LookupKey, Data = data };
        });

        var results = await Task.WhenAll(tasks);

        return new LookupResponse(results.ToDictionary(k => k.Key, v => v.Data));
    }
}