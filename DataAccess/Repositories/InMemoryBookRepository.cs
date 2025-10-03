using DataAccess.Repositories.Impl;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class InMemoryBookRepository : IBookRepository
{
	public async Task<IEnumerable<Book>> GetAllAsync(BooksDb db)
	{
		return await db.Books.ToListAsync();
	}

	public async Task<Book?> GetByIdAsync(Guid id, BooksDb db)
	{
		return await db.Books.FindAsync(id);
	}

	public async Task<Book> CreateAsync(Book book, BooksDb db)
	{
		db.Books.Add(book);
		await db.SaveChangesAsync();
		Console.WriteLine(book);

		return book;
	}

	public async Task<Book?> UpdateAsync(HttpContext context, string id, BooksDb db)
	{
		Guid.TryParse(id, out Guid guidId);
		if(Guid.Empty == guidId) return null;

		var book = await db.Books.FindAsync(guidId);
		if (book is null) return null;

		Book? source;

		if (context.Request.HasJsonContentType())
			source = await context.Request.ReadFromJsonAsync<Book>();
		else
			return null;

		if (source is not null)
		{
			book.Author = source.Author ?? book.Author;
			book.Title = source.Title ?? book.Title;
			book.ISBN = source.ISBN ?? book.ISBN;
			book.Year = source.Year > 1900 ? source.Year : book.Year;
			book.Price = source.Price > 0 ? source.Price : book.Price;
		}

		await db.SaveChangesAsync();

		return book;
	}

	public async Task<bool> DeleteAsync(Guid id, BooksDb db)
	{
		if (await db.Books.FindAsync(id) is not Book book) return false;

		db.Books.Remove(book);
		await db.SaveChangesAsync();
		return true;
	}
}