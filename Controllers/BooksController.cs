using BookAPIService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPIService.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService service, ILogger<BooksController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Searches for books using optional filters.
    /// </summary>
    /// <param name="title">Optional. Filters books by their title.</param>
    /// <param name="query">Optional. A general free-text search term that can match across multiple fields (e.g., title, author, description).</param>
    /// <returns>
    /// 200 OK — A JSON array of books matching the criteria.
    /// Possible error responses (depending on service implementation):
    /// - 400 Bad Request — If the query parameters are invalid.
    /// - 500 Internal Server Error — If the search service fails.
    /// </returns>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? title, [FromQuery] string? query)
    {
        _logger.LogInformation(
        "Search request received. Title={Title}, Query={Query}",
        title, query);
        var result = await _service.SearchBookAsync(title, query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of books filtered by subject.
    /// </summary>
    /// <param name="subject">Optional. The subject category to filter by. Default is <c>adventure</c>.</param>
    /// <param name="limit">Optional. Maximum number of items to return. Default is <c>10</c>.</param>
    /// <param name="offset">Optional. Number of items to skip for pagination. Default is <c>0</c>.</param>
    /// <returns>
    /// 200 OK — A JSON array of books for the given subject and pagination settings.
    /// 
    /// Possible error responses (depending on service implementation):
    /// - 400 Bad Request — If <c>limit</c> or <c>offset</c> are negative or exceed service constraints.
    /// - 500 Internal Server Error — If the listing service fails.
    /// </returns>
    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] string subject = "adventure", [FromQuery] int limit = 10,
     [FromQuery] int offset = 0)
    {
        _logger.LogInformation(
        "List request received. Subject={Subject}, Limit={Limit}, Offset={Offset}",
        subject, limit, offset);
        var result = await _service.ListBooksAsync(subject, limit, offset);
        return Ok(result);
    }

}