using KolBooks1.Models;
using KolBooks1.Repos;

namespace KolBooks1.Services;

public class BookService : IBookService
{
    private readonly IBookRepo _bookRepo;
    
    public BookService(IBookRepo bookRepo)
    {
        _bookRepo = bookRepo;
    }

    public async Task<BookDTO> GetAuthorsForBooksAsync(int id)
    {
        Book book = await _bookRepo.GetBookAsync(id);
        Author author = await _bookRepo.GetAuthorForBookAsync(id);
        AuthorDTO authorDto = new AuthorDTO(author.FirstName, author.LastName);
        BookDTO bookDto = new BookDTO(book.IdBook, book.Title, authorDto);

        return bookDto;
    }

    public async Task<Book2ResultDTO> AddBookAsync(Book2DTO book2Dto)
    {
        return await _bookRepo.AddNewBookWithAuthorAsync(book2Dto);
    }
}