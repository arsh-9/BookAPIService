using System.Text.Json.Serialization;

namespace BookAPIService.Models.OpenLibrary;

public class OpenLibraryListResponse
{
    [JsonPropertyName("works")]
    public List<OpenLibraryWork>? Works { get; set; }
}

public class OpenLibraryWork
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("first_publish_year")]
    public int? FirstPublishYear { get; set; }

    [JsonPropertyName("authors")]
    public List<OpenLibraryAuthorDto>? Authors { get; set; }

    [JsonPropertyName("subject")]
    public List<string>? Subjects { get; set; }

    [JsonPropertyName("cover_id")]
    public int? CoverId { get; set; }
}

public sealed class OpenLibraryAuthorDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}