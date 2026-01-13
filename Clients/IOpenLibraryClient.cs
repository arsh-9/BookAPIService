using BookAPIService.Models.OpenLibrary;

namespace BookAPIService.Clients;

public interface IOpenLibraryClient
{
    Task<OpenLibrarySearchResponse> SearchAsync(string searchKey, string searchValue, string fields, int limit);
    Task<OpenLibraryListResponse?> GetBySubjectAsync(string subject, int limit, int offset);
}