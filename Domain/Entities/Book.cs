using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public record Book
{
    // public Book(string? title, string? author, int? year, int? price)
    // {
    //     Id = Guid.NewGuid();
    //     Title = title;
    //     Author = author;
    // }
    public Guid Id {get; init;} = Guid.NewGuid();

    // [Required(ErrorMessage = "Book title is required")]
    // [StringLength(100, ErrorMessage = "The book title must not exceed 100 characters.")]
    public string? Title {get; set;}

    // [Required(ErrorMessage = "Book title is required")]
    // [StringLength(100, ErrorMessage = "The book author must not exceed 100 characters.")]
    public string? Author {get; set;}

    public string? ISBN { get; set; }

    // [Required(ErrorMessage = "Year is required")]
    [Range(1900, 2050, ErrorMessage = "The year must be between 1900 and 2050")]
    public int Year {get; set;}

    // [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0")]
    public decimal Price {get; set;}
}

