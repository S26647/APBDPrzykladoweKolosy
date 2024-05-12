using System.ComponentModel.DataAnnotations;

namespace KolBooks1.Models;

public class Author
{
    [Required]
    public int IdAuthor { get; set; }
    
    [MaxLength(50)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }
}