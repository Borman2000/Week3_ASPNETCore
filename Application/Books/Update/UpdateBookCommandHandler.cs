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
		var origBook = await bookRepository.GetByIdAsync(command.Id);
		if (origBook is null)
			return null;

		var book = new Book { Id = command.Id, Title = command.Title ?? origBook.Title, Price = command.Price ?? origBook.Price };

		var oldPrice = origBook.Price;

		await bookRepository.UpdateAsync(book);

// Move event adding to Infrastructure.Repositories.BookRepository.UpdateAsync ???
		if(oldPrice != book.Price)
			book.AddPriceUpdatedEvent(oldPrice);

		return dtoMapper.Map<BookDto>(book);
	}
}