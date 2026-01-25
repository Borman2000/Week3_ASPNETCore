using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Books.GetById;

public sealed record GetBookByIdQuery(Guid Id) : IRequest<BookDto?>, ICachableQuery
{
	public bool BypassCache => false;
	public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(10);

	public string CacheKey => $"Book_{Id}";
}