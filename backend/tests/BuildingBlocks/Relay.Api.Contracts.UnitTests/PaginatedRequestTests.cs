using System.ComponentModel.DataAnnotations;

namespace Relay.Api.Contracts.UnitTests;

public sealed class PaginatedRequestTests
{
    [Fact]
    public void Constructor_uses_expected_defaults()
    {
        // Act
        var request = new TestPaginatedRequest();

        // Assert
        Assert.Equal(PaginatedRequest.DefaultPage, request.Page);
        Assert.Equal(PaginatedRequest.DefaultPageSize, request.PageSize);
        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, PaginatedRequest.MaxPageSize)]
    [InlineData(int.MaxValue, 1)]
    public void Validate_returns_no_errors_when_pagination_is_valid(
        int page,
        int pageSize)
    {
        // Arrange
        var request = new TestPaginatedRequest(page, pageSize);

        // Act
        var results = Validate(request);

        // Assert
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Validate_returns_page_error_when_page_is_less_than_one(int page)
    {
        // Arrange
        var request = new TestPaginatedRequest(
            page,
            PaginatedRequest.DefaultPageSize);

        // Act
        var results = Validate(request);

        // Assert
        var result = Assert.Single(results);
        Assert.Contains(nameof(PaginatedRequest.Page), result.MemberNames);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [InlineData(PaginatedRequest.MaxPageSize + 1)]
    [InlineData(int.MaxValue)]
    public void Validate_returns_page_size_error_when_page_size_is_out_of_range(
        int pageSize)
    {
        // Arrange
        var request = new TestPaginatedRequest(page: 1, pageSize);

        // Act
        var results = Validate(request);

        // Assert
        var result = Assert.Single(results);
        Assert.Contains(nameof(PaginatedRequest.PageSize), result.MemberNames);
    }

    [Fact]
    public void Validate_returns_both_members_when_offset_exceeds_integer_limit()
    {
        // Arrange
        var request = new TestPaginatedRequest(int.MaxValue, pageSize: 2);

        // Act
        var results = Validate(request);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal(
            "The requested page is too large for the selected page size.",
            result.ErrorMessage);
        Assert.Equal(["pagination"], result.MemberNames);
    }

    private static IReadOnlyList<ValidationResult> Validate(
        PaginatedRequest request)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        return results;
    }

    private sealed class TestPaginatedRequest(
        int page = 1,
        int pageSize = PaginatedRequest.DefaultPageSize)
        : PaginatedRequest(page, pageSize);
}
