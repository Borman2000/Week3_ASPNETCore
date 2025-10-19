using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Domain.Base;

namespace Domain.Entities;

public class Category : CategoryBase
{
    public Category()
    {
Console.WriteLine("Category constructor 1");
    }

    [SetsRequiredMembers]
    public Category(string categoryName) : base(categoryName)
    {
Console.WriteLine("Category constructor 2");
    }

//    [SetsRequiredMembers]
//    public Category(string categoryName, string? categoryDescription) : base(categoryName, categoryDescription)
//    {
//Console.WriteLine("Category constructor 3");
//    }

    public List<Book> Books {get; set;} = new();
}
