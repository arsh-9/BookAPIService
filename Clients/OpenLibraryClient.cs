using BookAPIService.Exceptions;
using BookAPIService.Models.OpenLibrary;
using Microsoft.AspNetCore.WebUtilities;

namespace BookAPIService.Clients;

public class OpenLibraryClient : IOpenLibraryClient
{
    private readonly HttpClient _httpClient;

    public OpenLibraryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OpenLibrarySearchResponse> SearchAsync(string searchKey, string searchValue, string fields, int limit)
    {
        var queryParams = new Dictionary<string, string?>
        {
            [searchKey] = searchValue,
            ["limit"] = limit.ToString(),
            ["fields"] = string.Join(",", fields)
        };
        var url = QueryHelpers.AddQueryString("search.json", queryParams);
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalServiceException("Open Library search failed");
        }

        return await response.Content.ReadFromJsonAsync<OpenLibrarySearchResponse>();
    }

    public async Task<OpenLibraryListResponse?> GetBySubjectAsync(string subject, int limit, int offset)
    {
        var response = await _httpClient.GetAsync(
            $"subjects/{Uri.EscapeDataString(subject)}.json?limit={limit}&offset={offset}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<OpenLibraryListResponse>();
    }

}