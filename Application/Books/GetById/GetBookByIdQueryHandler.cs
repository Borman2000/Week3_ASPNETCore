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
//		var book = await bookRepository.AsNoTracking().Include(a => a.Author).AsNoTracking().Include(a => a.Categories).AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);
		var bookDto = dtoMapper.Map<BookDto>(book);
		return bookDto;
	}
}