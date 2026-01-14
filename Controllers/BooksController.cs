using BookAPIService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPIService.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService service,  ILogger<BooksController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? title, [FromQuery] string? query)
    {
        _logger.LogInformation(
        "Search request received. Title={Title}, Query={Query}",
        title, query);
        var result = await _service.SearchBookAsync(title, query);
        return Ok(result);
    }

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