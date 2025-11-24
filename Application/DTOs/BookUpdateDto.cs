using Domain.Entities;

namespace Application.DTOs;

public class BookUpdateDto(string? Title, Author Author, string? ISBN, int Year, decimal Price, string[] Categories) : BookDto(Title, ISBN, Year, Price, Categories);
