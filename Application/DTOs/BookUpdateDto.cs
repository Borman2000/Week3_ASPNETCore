using Domain.Entities;

namespace Application.DTOs;

public class BookUpdateDto(string? Title, Author Author, string? ISBN, int Year, decimal Price, string[] Categories) : BookDto(Title, ISBN, Year, Price, Categories);

// public record BookUpdateDto
// {
//     public string? Title {get;}
//     public Author Author {get;}
//     public string? ISBN { get; }
//     public int Year {get;}
//     public decimal Price {get;}
//     public List<Category> Categories { get; } = new();
// }