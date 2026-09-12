using System.ComponentModel.DataAnnotations;

namespace Relay.Api.Contracts;

public abstract class PaginatedRequest(int page, int pageSize)
    : IValidatableObject
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    [Range(1, int.MaxValue)]
    public int Page { get; } = page;

    [Range(1, MaxPageSize), Display(Name = "page_size")]
    public int PageSize { get; } = pageSize;

    public IEnumerable<ValidationResult> Validate(ValidationContext _)
    {
        var offset = ((long)Page - 1) * PageSize;

        if (offset > int.MaxValue)
        {
            yield return new ValidationResult(
                "The requested page is too large for the selected page size.",
                ["pagination"]);
        }
    }
}
