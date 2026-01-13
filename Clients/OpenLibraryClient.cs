using System.Net;
using System.Text.Json;
using BookAPIService.Models.Dtos;
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

        return await response.Content.ReadFromJsonAsync<OpenLibrarySearchResponse>();
    }

}