using BookAPIService.Models.Dtos;

namespace BookAPIService.Clients;

public interface IOpenLibraryClient
{
    Task<OpenLibrarySearchResponse> SearchAsync(string query);
}