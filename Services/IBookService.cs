
using BookAPIService.Models.Dtos;

namespace BookAPIService.Services;

public interface IBookService
{
    Task<IEnumerable<BookSearchDto>> SearchBooksAsync(string? title, string? isbn);
}