using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DataAccess.Repositories.Impl;

public interface IBookRepository
{
	Task<IEnumerable<Book>> GetAllAsync(BooksDb db);
	Task<Book?> GetByIdAsync(Guid id, BooksDb db);
	Task<Book> CreateAsync(Book book, BooksDb db);
	// Task<Book?> UpdateAsync(Book book, BooksDb db);
	Task<Book?> UpdateAsync(HttpContext context, string id, BooksDb db);
	Task<bool> DeleteAsync(Guid id, BooksDb db);
}
