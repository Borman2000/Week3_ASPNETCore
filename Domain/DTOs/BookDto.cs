using Domain.Base;
using Domain.Entities;

namespace Domain.DTOs;

public class BookDto : BookBase
{
    public BookDto()
    {
        Console.WriteLine("BookDto constructor 1");
    }

    public BookDto(string title, string isbn, int year, decimal price, string[] categories)
    {
Console.WriteLine("BookDto constructor 3");
        Title = title;
        ISBN = isbn;
        Year = year;
        Price = price;
        Categories = categories;
    }

    public Author Author { get; set; }

    public string[] Categories { get; set; }
}
