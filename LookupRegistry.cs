namespace Api.Lookups;

public record SimpleLookupDefinition(
    string TableName,
    string IdColumn,
    string LabelColumn,
    LookupMode Mode,
    string? FilterColumn = null,
    string? OrderBy = null,
    TimeSpan? CacheDuration = null
);

public class LookupRegistry
{
    private readonly Dictionary<string, SimpleLookupDefinition> _definitions 
        = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, SimpleLookupDefinition> Definitions => _definitions;

    public LookupRegistry()
    {
        // --- 1. STATIC DATA (Batched & Cached) ---
        
        Add("Countries", "Ref_Country", "IsoCode", "CountryName", 
            mode: LookupMode.Cached, 
            orderBy: "CountryName");

        // Dependent on CountryId
        Add("States", "Ref_State", "Id", "StateName", 
            mode: LookupMode.Cached, 
            filterColumn: "CountryId", 
            orderBy: "StateName");

        // --- 2. LIVE DATA (Search-as-you-type) ---
        
        Add("MedicalProviders", "MedicalProviders", "Id", "DisplayName", 
            mode: LookupMode.LiveSearch, 
            orderBy: "DisplayName");

        Add("Users", "AppUsers", "UserId", "FullName", 
            mode: LookupMode.LiveSearch, 
            orderBy: "FullName");
    }

    private void Add(string key, string table, string idCol, string labelCol, 
                     LookupMode mode,
                     string? filterColumn = null, string? orderBy = null)
    {
        _definitions[key] = new SimpleLookupDefinition(
            table, idCol, labelCol, mode, filterColumn, orderBy, TimeSpan.FromMinutes(60));
    }
}
