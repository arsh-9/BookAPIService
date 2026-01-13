using BookAPIService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPIService.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;

    public BooksController(IBookService service)
    {
        _service = service;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? title, [FromQuery] string? query)
    {
        var result = await _service.SearchBookAsync(title, query);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] string subject = "adventure", [FromQuery] int limit = 10,
     [FromQuery] int offset = 0)
    {
        var result = await _service.ListBooksAsync(subject, limit, offset);
        return Ok(result);
    }

}