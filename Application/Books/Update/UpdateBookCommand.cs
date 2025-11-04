using Application.DTOs;
using MediatR;

namespace Application.Books.Update;

public record UpdateBookCommand(Guid Id, string Title, decimal Price) : IRequest<BookDto?>;