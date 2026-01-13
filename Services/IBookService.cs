
using BookAPIService.Models.Dtos;

namespace BookAPIService.Services;

public interface IBookService
{
    Task<IEnumerable<BookSearchDto>> SearchBookAsync(string? title, string? query);
    Task<IEnumerable<BookListDto>> ListBooksAsync(string subject, int limit, int offset);
}