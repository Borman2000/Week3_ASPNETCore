using AutoMapper;
using DataAccess.Repositories.Impl;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class BookRepository(BookStoreDbContext dbContext) : EfRepository<Book, BookDto>(dbContext), IBookRepository
{
	public new async Task<IEnumerable<Book>> GetAllAsync()
	{
//		return await DbSet.Include(entity => entity.Author).ToListAsync();
		return await DbSet.Include(entity => entity.Categories).ToListAsync();
	}

	public new async Task<BookDto?> GetByIdAsync(Guid id, IMapper mapper)
	{
		var book = await DbSet.Include(a => a.Author).Include(a => a.Categories).SingleOrDefaultAsync(a => a.Id == id);
		var bookDto = mapper.Map<BookDto>(book);
		return bookDto;
	}

	public new async Task<Book?> AddAsync(BookDto bookDto)
    {
//        var book2 = mapper.Map<Book>(bookDto);
//        await DbContext.Books.AddAsync(book2);
//
//		await DbContext.SaveChangesAsync();
//
//        return book2;
        var book = await DbContext.Books.FirstOrDefaultAsync(a => a.Title == bookDto.Title);
        if (book is not null)
			return null;	//Results.Conflict("Book already exists");

        var bk = (BookCreateDto)bookDto;
        var author = await DbContext.Authors.FirstOrDefaultAsync(a => a.FirstName == bk.FirstName && a.LastName == bk.LastName);
        if (author == null)
        {
            author = new Author(bk.FirstName, bk.LastName, DateTime.MinValue);
            DbContext.Authors.Add(author);
        }

        // var dbCategories = await DbContext.Categories.Where(c => bookDto.Categories.Contains(c.Name)).Select(c => c.Name).ToListAsync();
        var dbCategories = await DbContext.Categories.Where(c => bookDto.Categories.Contains(c.Name)).ToListAsync();
        if (dbCategories.Count != bookDto.Categories.Length)
        {
            var newCategories = bookDto.Categories.Except(dbCategories.Select(c => c.Name));
            List<Category> lstCategories = [];
            foreach (var category in newCategories)
                lstCategories.Add(new Category(category));

            await DbContext.Categories.AddRangeAsync(lstCategories);

            dbCategories = dbCategories.Concat(lstCategories).ToList();
        }
        if (book == null)
        {
            book = new Book
            {
                Title = bookDto.Title,
                Author = author,
                ISBN = bookDto.ISBN,
                // AuthorId = author.Id,
                Year = bookDto.Year,
                Price = bookDto.Price,
                Categories = dbCategories
            };
            await DbContext.Books.AddAsync(book);
        }

		await DbContext.SaveChangesAsync();
		// Console.WriteLine(book);

		return null;
	}

    public async Task<Book?> UpdateAsync(BookUpdateDto newBook)
	{
		// Guid.TryParse(id, out Guid guidId);
		// if(Guid.Empty == guidId) return null;
		//
		var book = await DbContext.Books.FindAsync("");
		// if (book is null) return null;
		//
		// Book? source;
		//
		// if (context.Request.HasJsonContentType())
		// 	source = await context.Request.ReadFromJsonAsync<Book>();
		// else
		// 	return null;
		//
		// if (source is not null)
		// {
		// 	book.Author = source.Author ?? book.Author;
		// 	book.Title = source.Title ?? book.Title;
		// 	book.ISBN = source.ISBN ?? book.ISBN;
		// 	book.Year = source.Year > 1900 ? source.Year : book.Year;
		// 	book.Price = source.Price > 0 ? source.Price : book.Price;
		// }

		await DbContext.SaveChangesAsync();

		return book;
	}

	public async Task<bool> DeleteAsync(Guid id)
	{
		if (await DbContext.Books.FindAsync(id) is not Book book) return false;

        DbContext.Books.Remove(book);
		await DbContext.SaveChangesAsync();
		return true;
	}
}