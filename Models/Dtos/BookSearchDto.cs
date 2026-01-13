namespace BookAPIService.Models.Dtos;

public class BookSearchDto
{
    public string Title { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public string Key { get; set; } = string.Empty;
}