using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Base;

public class CategoryBase : BaseEntity
{
    public CategoryBase()
    {
    }

    [SetsRequiredMembers]
    public CategoryBase(string categoryName)
    {
        Name = categoryName;
    }

    [SetsRequiredMembers]
    public CategoryBase(string categoryName, string? categoryDescription)
    {
        Name = categoryName;
        Description = categoryDescription;
    }

    [Required(ErrorMessage = "Category name is required")]
    [StringLength(20, ErrorMessage = "Category name must not exceed 20 characters.")]
    public string Name {get; set;}

    public string? Description { get; set; }
}
