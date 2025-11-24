using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Books.GetById;

public class GetBookByIdQueryHandler(IBookRepository bookRepository, IMapper dtoMapper) : IRequestHandler<GetBookByIdQuery, BookDto?>
{
	public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
	{
		var book = await bookRepository.GetByIdAsync(request.Id);
		var bookDto = dtoMapper.Map<BookDto>(book);
		return bookDto;
	}
}