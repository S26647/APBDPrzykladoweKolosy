using KolBooks1.Models;

namespace KolBooks1.Services;

public interface IBookService
{
    Task<BookDTO> GetAuthorsForBooksAsync(int id);

    Task<Book2ResultDTO> AddBookAsync(Book2DTO book2Dto);
}