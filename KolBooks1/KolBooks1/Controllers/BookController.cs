using KolBooks1.Models;
using KolBooks1.Services;
using Microsoft.AspNetCore.Mvc;

namespace KolBooks1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet("{id}/authors")]
    public async Task<IActionResult> GetAuthorsForBookAsync(int id)
    {
        BookDTO bookDto = await _bookService.GetAuthorsForBooksAsync(id);

        return Ok(bookDto);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddBookAsync(Book2DTO book2Dto)
    {
        var book = await _bookService.AddBookAsync(book2Dto);
        if (book == null)
            return StatusCode(StatusCodes.Status400BadRequest);
        return Ok(book);
    }
}