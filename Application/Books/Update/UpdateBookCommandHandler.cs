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

		var origBook = await bookRepository.GetByIdAsync(book.Id);
		var oldPrice = command.Price;
		if (origBook is not null)
			oldPrice = origBook.Price;

		await bookRepository.UpdateAsync(book);

// Move event adding to Infrastructure.Repositories.BookRepository.UpdateAsync ???
		if(oldPrice != book.Price)
			book.AddPriceUpdatedEvent(oldPrice);

		return dtoMapper.Map<BookDto>(book);
	}
}