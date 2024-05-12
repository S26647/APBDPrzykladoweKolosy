using KolBooks1.Models;

namespace KolBooks1.Repos;

public interface IBookRepo
{
    Task<Author> GetAuthorForBookAsync(int idBook);
    Task<Book> GetBookAsync(int idBook);

    Task<int?> GetAuthorIdByFNameAndLNameAsync(string firstName, string lastName);

    Task<Book2ResultDTO> AddNewBookWithAuthorAsync(Book2DTO book2Dto);
}