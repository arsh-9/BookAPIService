using System.Text.Json.Serialization;

namespace BookAPIService.Models.Dtos;

public class OpenLibrarySearchResponse
{
    [JsonPropertyName("docs")]
    public List<OpenLibraryDoc>? Docs { get; set; }
}

public class OpenLibraryDoc
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("cover_i")]
    public int? CoverId { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }
}