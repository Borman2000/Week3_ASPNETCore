using Domain.Base;
using Domain.Entities;

namespace Application.DTOs;

public class BookDto : BookBase
{
    public BookDto()
    {
    }

    public BookDto(string title, string isbn, int year, decimal price, string[] categories)
    {
        Title = title;
        ISBN = isbn;
        Year = year;
        Price = price;
        Categories = categories;
    }

    public Author Author { get; set; }

    public string[] Categories { get; set; }
}
