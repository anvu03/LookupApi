namespace Api.Lookups;

// 1. Request DTO
public record LookupRequest(
    string LookupKey,
    string? SearchTerm = null,
    Dictionary<string, object>? Parameters = null,
    int Page = 1,
    int PageSize = 20
);

// 2. Response Item
public record LookupItem(string Value, string Label, object? Metadata = null);

// 3. Paged Result Wrapper
public record PagedLookupResult(
    IEnumerable<LookupItem> Items,
    int TotalCount,
    bool HasMore
);

// 4. Top Level API Response
public record LookupResponse(Dictionary<string, PagedLookupResult> Data);

// 5. Configuration Enums
public enum LookupMode
{
    Cached,    // Fetch all, cache, filter in-memory (Countries, States)
    LiveSearch // SQL Paging, no cache, search via WHERE clause (Providers, Users)
}
