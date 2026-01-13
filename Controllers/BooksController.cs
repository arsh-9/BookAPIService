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
    public async Task<IActionResult> Search([FromQuery] string? title, [FromQuery] string? isbn)
    {
        var result = await _service.SearchBooksAsync(title, isbn);
        return Ok(result);
    }

}