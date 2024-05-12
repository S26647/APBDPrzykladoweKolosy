using System.ComponentModel.DataAnnotations;

namespace KolBooks1.Models;

public class Book
{
    [Required]
    public int IdBook { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
}