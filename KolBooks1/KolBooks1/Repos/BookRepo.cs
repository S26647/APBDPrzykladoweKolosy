using System.Data.Common;
using System.Data.SqlClient;
using KolBooks1.Models;

namespace KolBooks1.Repos;

public class BookRepo : IBookRepo
{
    private IConfiguration _configuration;

    public BookRepo(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Author> GetAuthorForBookAsync(int IdBook)
    {
        using SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        con.OpenAsync();
        
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM authors " +
                          "JOIN books_authors ON books_authors.FK_author = authors.PK " +
                          "WHERE books_authors.FK_book = @IdBook";
        cmd.Parameters.AddWithValue("@IdBook", IdBook);
        
        using (var dr = await cmd.ExecuteReaderAsync())
        {
            while (await dr.ReadAsync())
            {
                return new Author
                {
                    IdAuthor = Int32.Parse(dr["PK"].ToString()),
                    FirstName = dr["first_name"].ToString(),
                    LastName = dr["last_name"].ToString()
                };
            }
        }

        return null;
    }

    public async Task<Book> GetBookAsync(int IdBook)
    {
        using SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM books " +
                          "WHERE books.PK = @IdBook";
        cmd.Parameters.AddWithValue("@IdBook", IdBook);
        
        using (var dr = await cmd.ExecuteReaderAsync())
        {
            while (await dr.ReadAsync())
            {
                return new Book
                {
                    IdBook = Int32.Parse(dr["PK"].ToString()),
                    Title = dr["title"].ToString()
                };
            }
        }

        return null;
    }

    public async Task<int?> GetAuthorIdByFNameAndLNameAsync(string firstName, string lastName)
    {
        await using SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT PK FROM authors " +
                          "WHERE authors.first_name = @firstName " +
                          "AND authors.last_name = @lastName";
        cmd.Parameters.AddWithValue("@firstName", firstName);
        cmd.Parameters.AddWithValue("@lastName", lastName);

        await using (var dr = await cmd.ExecuteReaderAsync())
        {
            while (await dr.ReadAsync())
            {
                return Int32.Parse(dr["PK"].ToString());
            }
        }

        return null;
    }

    public async Task<Book2ResultDTO> AddNewBookWithAuthorAsync(Book2DTO book2Dto)
    {
        using SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var bookCmd = new SqlCommand();
        bookCmd.Connection = con;
        bookCmd.CommandText = "INSERT INTO books (title) VALUES (@BookTitle); SELECT SCOPE_IDENTITY() as PK;";
        bookCmd.Parameters.AddWithValue("@BookTitle", book2Dto.Title);
        
        var dr = await bookCmd.ExecuteReaderAsync();
        await dr.ReadAsync();
        var bookId = Int32.Parse(dr["PK"].ToString());
        await dr.CloseAsync();
        
        await bookCmd.ExecuteNonQueryAsync();

        foreach (var author in book2Dto.AuthorDto)
        { 
            int? authorId = await GetAuthorIdByFNameAndLNameAsync(author.FirstName, author.LastName);
            if (authorId == null)
            {
                await using var authorsCmd = new SqlCommand();
                authorsCmd.Connection = con;
                authorsCmd.CommandText = "INSERT INTO authors (first_name, last_name) VALUES (@FirstName, @LastName); SELECT SCOPE_IDENTITY() as PKauthor;";
                authorsCmd.Parameters.AddWithValue("@FirstName", author.FirstName);
                authorsCmd.Parameters.AddWithValue("@LastName", author.LastName);
                
                var drAuthor = await bookCmd.ExecuteReaderAsync();
                await drAuthor.ReadAsync();
                authorId = Int32.Parse(drAuthor["PKauthor"].ToString());
                await drAuthor.CloseAsync();
                
                await authorsCmd.ExecuteNonQueryAsync();
            }
            await using var booksAuthorsCmd = new SqlCommand();
            booksAuthorsCmd.Connection = con;
            booksAuthorsCmd.CommandText = "INSERT INTO books_authors (FK_book, FK_author) VALUES (@BookId, @AuthorId);";
            booksAuthorsCmd.Parameters.AddWithValue("@BookId", bookId);
            booksAuthorsCmd.Parameters.AddWithValue("@AuthorId", authorId);
            await booksAuthorsCmd.ExecuteNonQueryAsync();
        }
        
        return new Book2ResultDTO(bookId, book2Dto.Title, book2Dto.AuthorDto);
    }
}