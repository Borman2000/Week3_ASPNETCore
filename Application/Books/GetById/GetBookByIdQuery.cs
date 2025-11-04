using Application.DTOs;
using MediatR;

namespace Application.Books.GetById;

public sealed record GetBookByIdQuery(Guid Id) : IRequest<BookDto?>;