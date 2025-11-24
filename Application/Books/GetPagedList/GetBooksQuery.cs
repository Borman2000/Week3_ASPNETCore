using Application.DTOs;
using MediatR;

namespace Application.Books.GetPagedList;

public record GetBooksQuery(int? Page, int? PageSize, string? SearchTerm) : IRequest<List<BookDto?>>;