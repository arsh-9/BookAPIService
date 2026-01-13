using BookAPIService.Clients;
using BookAPIService.Models.Dtos;

namespace BookAPIService.Services;

public class BookService : IBookService
{
    private readonly IOpenLibraryClient _client;

    public BookService(IOpenLibraryClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<BookSearchDto>> SearchBooksAsync(
        string? title,
        string? isbn)
    {
        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("Either title or isbn must be provided.");

        // ISBN takes precedence if available
        var isIsbnSearch = !string.IsNullOrWhiteSpace(isbn);

        var response = await _client.SearchAsync(
            searchKey: isIsbnSearch ? "isbn" : "title",
            searchValue: isIsbnSearch ? isbn! : title!,
            limit: 1,
            fields: OpenLibraryFields.BookSearchFields);

        return response.Docs?
        .Select(doc => new BookSearchDto
        {
            Title = doc.Title,
            Key = doc.Key,
            CoverUrl = doc.CoverId.HasValue
                ? $"https://covers.openlibrary.org/b/id/{doc.CoverId}-L.jpg"
                : null
        }) ?? Enumerable.Empty<BookSearchDto>();
    }
}

