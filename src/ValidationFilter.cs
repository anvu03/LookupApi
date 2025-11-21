using FluentValidation;

namespace Api.Lookups;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // 1. Find the validator in DI
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is not null)
        {
            // 2. Find the argument in the request (e.g., the List<LookupRequest>)
            var entity = context.Arguments
                .OfType<T>()
                .FirstOrDefault(a => a?.GetType() == typeof(T));

            if (entity is not null)
            {
                // 3. Validate
                var validation = await validator.ValidateAsync(entity);
                if (!validation.IsValid)
                {
                    // 4. Return standard Dictionary<string, string[]> error format
                    return TypedResults.ValidationProblem(validation.ToDictionary());
                }
            }
        }

        // 5. Continue pipeline
        return await next(context);
    }
}