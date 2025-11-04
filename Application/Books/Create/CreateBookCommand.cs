using Application.DTOs;
using MediatR;

namespace Application.Books.Create;

public record CreateBookCommand(string Title, string ISBN, Guid AuthorId, decimal Price) : IRequest<BookDto?>;