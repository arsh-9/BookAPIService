using BookAPIService.Clients;
using BookAPIService.Models.Dtos;
using BookAPIService.Models.OpenLibrary;

namespace BookAPIService.Services;

public class BookService : IBookService
{
    private readonly IOpenLibraryClient _client;

    public BookService(IOpenLibraryClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<BookSearchDto>> SearchBookAsync(
        string? title,
        string? query)
    {
        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Either title or query must be provided.");

        // Title takes precedence if available
        var isTitleSearch = !string.IsNullOrWhiteSpace(title);

        var response = await _client.SearchAsync(
            searchKey: isTitleSearch ? "title" : "q",
            searchValue: isTitleSearch ? title : query,
            limit: 1,
            fields: OpenLibraryFields.BookSearchFields);

        return MapBookResponse(response);
    }

    private IEnumerable<BookSearchDto> MapBookResponse(OpenLibrarySearchResponse response)
    {
        if (response?.Docs == null)
            return Enumerable.Empty<BookSearchDto>();

        return response.Docs.Select(doc => new BookSearchDto
        {
            Title = doc.Title ?? string.Empty,
            Key = doc.Key,
            CoverUrl = doc.CoverId.HasValue
                ? $"https://covers.openlibrary.org/b/id/{doc.CoverId}-L.jpg"
                : null
        });
    }

    public async Task<IEnumerable<BookListDto>> ListBooksAsync(string subject, int limit, int offset)
    {
        limit = Math.Min(limit <= 0 ? 10 : limit, 10);
        offset = Math.Max(offset, 0);

        var response = await _client.GetBySubjectAsync(subject, limit, offset);

        return MapBookListResponse(response);
    }

    private IEnumerable<BookListDto> MapBookListResponse(OpenLibraryListResponse response)
    {
        if (response?.Works?.Any() != true)
        {
            return Enumerable.Empty<BookListDto>();
        }

        return response.Works.Select(w => new BookListDto
        {
            Title = w.Title ?? string.Empty,
            PublishYear = w.FirstPublishYear,
            Authors = string.Join(", ",
                w.Authors?
                    .Select(a => a.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n)) ?? Enumerable.Empty<string>()),

            Subjects = string.Join(", ",
                w.Subjects?
                    .Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>()),

            CoverUrl = w.CoverId.HasValue
                ? $"https://covers.openlibrary.org/b/id/{w.CoverId}-M.jpg"
                : null
        });
    }
}

