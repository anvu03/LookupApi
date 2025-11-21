using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;

namespace Api.Lookups;

public interface ILookupSource
{
    Task<PagedLookupResult> FetchAsync(LookupRequest request, CancellationToken ct);
}

record CachedLookupItem(LookupItem Item, string? FilterVal);

public class GenericDapperLookupHandler : ILookupSource
{
    private readonly string _connString;
    private readonly LookupRegistry _registry;
    private readonly HybridCache _cache;

    public GenericDapperLookupHandler(
        IConfiguration config, 
        LookupRegistry registry, 
        HybridCache cache)
    {
        _connString = config.GetConnectionString("DefaultConnection")!;
        _registry = registry;
        _cache = cache;
    }

    public async Task<PagedLookupResult> FetchAsync(LookupRequest request, CancellationToken ct)
    {
        if (!_registry.Definitions.TryGetValue(request.LookupKey, out var def)) 
            return new PagedLookupResult(Enumerable.Empty<LookupItem>(), 0, false);

        // --- STRATEGY A: CACHED (Memory) ---
        if (def.Mode == LookupMode.Cached)
        {
            // 1. Fetch or Get from HybridCache (L1 + L2 + Stampede Protection)
            var allItems = await _cache.GetOrCreateAsync($"lookup:{request.LookupKey}", async token => 
            {
                using var conn = new SqlConnection(_connString);
                var sql = $"SELECT CAST({def.IdColumn} AS NVARCHAR(50)) as Value, {def.LabelColumn} as Label, {def.FilterColumn ?? "NULL"} as FilterVal FROM {def.TableName}";
                if (def.OrderBy != null) sql += $" ORDER BY {def.OrderBy}";

                var result = await conn.QueryAsync(sql); 
                return result.Select(x => new CachedLookupItem(
                    new LookupItem((string)x.Value, (string)x.Label), 
                    x.FilterVal?.ToString()
                )).ToList();
            }, cancellationToken: ct);

            // 2. Filter In-Memory
            var query = allItems.AsEnumerable();

            if (def.FilterColumn != null && request.Parameters != null && 
                request.Parameters.TryGetValue(def.FilterColumn, out var pVal))
            {
                var filterValue = GetParameterValue(pVal);
                query = query.Where(x => x.FilterVal == filterValue);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => x.Item.Label.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Return ALL (Ignore pagination for static lists)
            var final = query.Select(x => x.Item).ToList();
            return new PagedLookupResult(final, final.Count, false);
        }

        // --- STRATEGY B: LIVE (SQL Paging) ---
        else 
        {
            int offset = (request.Page - 1) * request.PageSize;
            using var conn = new SqlConnection(_connString);
            var builder = new SqlBuilder();

            // Define Templates
            var selector = builder.AddTemplate(
                $"SELECT CAST({def.IdColumn} AS NVARCHAR(50)) AS Value, {def.LabelColumn} AS Label " +
                $"FROM {def.TableName} /**where**/ /**orderby**/ " +
                $"OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", 
                new { Offset = offset, PageSize = request.PageSize });

            var counter = builder.AddTemplate($"SELECT COUNT(*) FROM {def.TableName} /**where**/");

            // Dynamic Filtering
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                builder.Where($"{def.LabelColumn} LIKE @Search", new { Search = $"%{request.SearchTerm}%" });

            if (def.FilterColumn != null && request.Parameters != null && 
                request.Parameters.TryGetValue(def.FilterColumn, out var pVal))
            {
                var filterValue = GetParameterValue(pVal);
                builder.Where($"{def.FilterColumn} = @FilterVal", new { FilterVal = filterValue });
            }

            builder.OrderBy(def.OrderBy ?? def.LabelColumn);

            // Execute
            using var multi = await conn.QueryMultipleAsync(
                $"{selector.RawSql}; {counter.RawSql}", selector.Parameters);

            var items = await multi.ReadAsync<LookupItem>();
            var totalCount = await multi.ReadFirstAsync<int>();

            return new PagedLookupResult(items, totalCount, (offset + request.PageSize) < totalCount);
        }
    }

    private static string? GetParameterValue(object? value)
    {
        if (value is null) return null;
        
        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => jsonElement.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => null,
                _ => jsonElement.ToString()
            };
        }
        
        return value.ToString();
    }
}