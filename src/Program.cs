using Api.Lookups;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// 1. INFRASTRUCTURE & CROSS-CUTTING CONCERNS
// =============================================================================

// Enable .NET 9 HybridCache (L1 Memory + L2 Distributed + Stampede Protection)
builder.Services.AddHybridCache(options => 
{
    // Optional: Global defaults
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(60),
        LocalCacheExpiration = TimeSpan.FromMinutes(60)
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auto-register all FluentValidation validators in this assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// =============================================================================
// 2. LOOKUP DOMAIN SERVICES
// =============================================================================

// The Configuration Registry (Singleton: It's static definition data)
builder.Services.AddSingleton<LookupRegistry>();

// The "Engine" that executes Dapper queries (Singleton: Stateless)
builder.Services.AddSingleton<GenericDapperLookupHandler>();

// The "Traffic Cop" that manages the batch logic (Scoped: Resolves other services)
builder.Services.AddScoped<LookupOrchestrator>();

// OPTIONAL: If you ever need custom logic (Scenario C), register it here:
// builder.Services.AddKeyedScoped<ILookupSource, ComplexUserHandler>("Users");

var app = builder.Build();

// =============================================================================
// 3. HTTP PIPELINE
// =============================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register the Lookup Module (Keeps Program.cs clean)
app.MapLookupEndpoints();

app.Run();