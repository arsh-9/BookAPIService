using BookAPIService.Clients;
using BookAPIService.Models.Dtos;
using BookAPIService.Models.OpenLibrary;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BookAPIService.Services;

public class BookService : IBookService
{
    private readonly IOpenLibraryClient _client;
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<BookService> _logger;

    public BookService(IOpenLibraryClient client, IMemoryCache cache, IOptions<CacheOptions> cacheOptions,
    ILogger<BookService> logger)
    {
        _client = client;
        _cache = cache;
        _cacheOptions = cacheOptions.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<BookSearchDto>> SearchBookAsync(
        string? title,
        string? query)
    {
        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Either title or query must be provided.");

        // Title takes precedence if available
        var isTitleSearch = !string.IsNullOrWhiteSpace(title);

        _logger.LogInformation("Executing search with {SearchType}={SearchValue}",
            isTitleSearch ? "title" : "query",
            isTitleSearch ? title : query);

        var response = await _client.SearchAsync(
            searchKey: isTitleSearch ? "title" : "q",
            searchValue: isTitleSearch ? title : query,
            limit: 1,
            fields: OpenLibraryFields.BookSearchFields);

        var book = MapBookResponse(response);

        _logger.LogInformation("Book search completed with {Result}",
            book.Count() == 0 ? "NoMatches" : "MatchesFound");

        return book;
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
        var cacheKey = BuildCacheKey(subject, limit, offset);
        if (_cache.TryGetValue(cacheKey, out IEnumerable<BookListDto> cached))
        {
            _logger.LogInformation("Cache hit for key {CacheKey}", cacheKey);
            return cached;
        }
        _logger.LogInformation("Cache miss for key {CacheKey}", cacheKey);

        var response = await _client.GetBySubjectAsync(subject, limit, offset);
        var bookLists = MapBookListResponse(response);
        if (bookLists.Any())
        {
            _cache.Set(
            cacheKey,
            bookLists,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.ListCacheMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(_cacheOptions.SlidingCacheMinutes)
            });
            _logger.LogInformation(
            "Cached {Count} books for key {CacheKey}",
            bookLists.Count(),
            cacheKey);
        }

        _logger.LogInformation("Fetched {Count} Books from external API", bookLists.Count());
        return bookLists;
    }

    private static string BuildCacheKey(string subject, int limit, int offset)
        => $"books:list:{subject.ToLower()}:{limit}:{offset}";

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

