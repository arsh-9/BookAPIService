using System.Net;
using System.Text.Json;
using BookAPIService.Models.Dtos;

namespace BookAPIService.Clients;

public class OpenLibraryClient : IOpenLibraryClient
{
    private readonly HttpClient _httpClient;

    public OpenLibraryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OpenLibrarySearchResponse> SearchAsync(string query)
    {
        var response = await _httpClient.GetAsync(
            $"search.json?q={Uri.EscapeDataString(query)}");

        return await response.Content.ReadFromJsonAsync<OpenLibrarySearchResponse>();
    }

}