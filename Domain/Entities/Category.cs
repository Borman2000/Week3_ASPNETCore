using System.Diagnostics.CodeAnalysis;
using Domain.Base;

namespace Domain.Entities;

public class Category : CategoryBase
{
    public Category()
    {
    }

    [SetsRequiredMembers]
    public Category(string categoryName) : base(categoryName)
    {
    }

    public List<Book> Books {get; set;} = new();
}
