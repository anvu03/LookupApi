using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc; // Only for [FromBody] if needed explicitly

namespace Api.Lookups;

public static class LookupEndpoints
{
    public static void MapLookupEndpoints(this IEndpointRouteBuilder app)
    {
        // Define a Versioned Group
        var group = app.MapGroup("/api/v1/lookups")
            .WithTags("Reference Data");

        // Map the specific route
        group.MapPost("/", GetLookupsHandler)
             .WithName("GetBatchLookups")
             .WithSummary("Retrieves a batch of reference data (cached or live).")
             .AddEndpointFilter<ValidationFilter<List<LookupRequest>>>(); // Apply Validation
    }

    // The Handler Method
    // Note: logic is delegating strictly to the Orchestrator
    private static async Task<Results<Ok<LookupResponse>, BadRequest<string>>> GetLookupsHandler(
        [FromBody] List<LookupRequest> requests,
        [FromServices] LookupOrchestrator orchestrator,
        CancellationToken ct)
    {
        // Guard clause is handled by the ValidationFilter, but safe to keep basics
        if (requests.Count == 0)
        {
            return TypedResults.BadRequest("Batch cannot be empty.");
        }

        var result = await orchestrator.ProcessBatchAsync(requests, ct);
        
        return TypedResults.Ok(result);
    }
}