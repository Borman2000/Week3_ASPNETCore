using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Books.Update;

public record UpdateBookCommand(Guid Id, string? Title, decimal? Price) : IRequest<BookDto?>, ICacheInvalidation
{
	public IEnumerable<string> CacheKeys => new []{$"Book_{Id}"};
}