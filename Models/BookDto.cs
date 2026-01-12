public class BookDto
{
    public string Title { get; set; } = string.Empty;
    public int? PublishYear { get; set; }
    public string Authors { get; set; } = string.Empty;
    public string Subjects { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
}