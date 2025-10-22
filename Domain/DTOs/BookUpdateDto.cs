using Domain.Entities;

namespace Domain.DTOs;

public class BookUpdateDto(string? Title,  Author Author, string? ISBN, int Year, decimal Price, List<Category> Categories) : BookDto;

// public record BookUpdateDto
// {
//     public string? Title {get;}
//     public Author Author {get;}
//     public string? ISBN { get; }
//     public int Year {get;}
//     public decimal Price {get;}
//     public List<Category> Categories { get; } = new();
// }