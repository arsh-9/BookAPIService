namespace BookAPIService.Models.Errors;
public sealed class ApiError
{
    public string Title { get; init; } = default!;
    public int Status { get; init; }
    public string Detail { get; init; } = default!;
}
