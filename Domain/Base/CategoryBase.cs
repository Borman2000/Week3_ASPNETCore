using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Base;

public class CategoryBase : BaseEntity
{
    public CategoryBase()
    {
Console.WriteLine("Category constructor 1");
    }

    [SetsRequiredMembers]
    public CategoryBase(string categoryName)
    {
Console.WriteLine("Category constructor 2");
        Name = categoryName;
    }

    [SetsRequiredMembers]
    public CategoryBase(string categoryName, string? categoryDescription)
    {
Console.WriteLine("Category constructor 3");
        Name = categoryName;
        Description = categoryDescription;
    }

    [Required(ErrorMessage = "Category name is required")]
    [StringLength(20, ErrorMessage = "Category name must not exceed 20 characters.")]
    public required string Name {get; set;}

    public string? Description { get; set; }
}
