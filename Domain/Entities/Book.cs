using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace Domain.Entities;

public class Book : BookBase
{
    [Required(ErrorMessage = "Book author is required")]
    public required Author Author {get; set;}

    [Required(ErrorMessage = "Category is required")]
    [Length(1, 10, ErrorMessage = "Should have at least one category")]
    public required List<Category> Categories { get; set; }

    public Guid AuthorId {get; set;}
}
