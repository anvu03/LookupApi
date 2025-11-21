using FluentValidation;

namespace Api.Lookups;

public class LookupBatchValidator : AbstractValidator<List<LookupRequest>>
{
    public LookupBatchValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Request list cannot be null or empty.")
            .Must(x => x.Count <= 10).WithMessage("Cannot request more than 10 lookups in a single batch."); // Performance Guardrail

        RuleForEach(x => x).SetValidator(new LookupRequestValidator());
    }
}

public class LookupRequestValidator : AbstractValidator<LookupRequest>
{
    public LookupRequestValidator()
    {
        RuleFor(x => x.LookupKey)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Lookup Key contains invalid characters."); // Injection Prevention

        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100); // Pagination Guardrail
    }
}