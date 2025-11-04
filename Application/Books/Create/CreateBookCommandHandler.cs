using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Books.Create;

public class CreateBookCommandHandler(IBookRepository bookRepository, IMapper dtoMapper) : IRequestHandler<CreateBookCommand, BookDto?>
{
	public async Task<BookDto?> Handle(CreateBookCommand command, CancellationToken cancellationToken)
	{
		var book = await bookRepository.AddAsync(new Book{AuthorId =  command.AuthorId, Title = command.Title, Price = command.Price, ISBN = command.ISBN});
		return dtoMapper.Map<BookDto>(book);
	}
}