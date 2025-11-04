using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Books.Update;

public class UpdateBookCommandHandler(IBookRepository bookRepository, IMapper dtoMapper) : IRequestHandler<UpdateBookCommand, BookDto?>
{
	public async Task<BookDto?> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
	{
		var book = new Book { Id = command.Id, Title = command.Title, Price = command.Price };
		await bookRepository.UpdateAsync(book);
		return dtoMapper.Map<BookDto>(book);
	}
}