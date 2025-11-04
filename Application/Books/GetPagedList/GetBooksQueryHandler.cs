using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Books.GetPagedList;

public class GetBooksQueryHandler(IBookRepository bookRepository, IMapper dtoMapper) : IRequestHandler<GetBooksQuery, List<BookDto?>>
{
	public async Task<List<BookDto?>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
	{
		var books = await bookRepository.Search(request.Page ?? 1, request.PageSize ?? 10, request.SearchTerm, null, null, null, null);
		return dtoMapper.Map<List<BookDto?>>(books);
	}
}