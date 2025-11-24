using System.ComponentModel.DataAnnotations;
using Domain.Validations;

namespace Domain.Base;

public class BookBase : BaseEntity
{
    [Required(ErrorMessage = "Book title is required")]
    [StringLength(100, ErrorMessage = "The book title must not exceed 100 characters.")]
    public string Title {get; set;}

    [IsbnValidation]
    [Required(ErrorMessage = "Book ISBN is required")]
    [StringLength(17, MinimumLength = 10, ErrorMessage = "The ISBN length must be between 10 and 17 symbols.")]
    public string ISBN { get; set; }

    [Required(ErrorMessage = "Year is required")]
    [Range(1900, 2050, ErrorMessage = "The year must be between 1900 and 2050")]
    public int Year {get; set;}

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0")]
    public decimal Price {get; set;}
}

