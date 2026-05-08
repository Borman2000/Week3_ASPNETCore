using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BookRepository(BookStoreDbContext dbContext, IMapper dtoMapper) : EfRepository<Book>(dbContext, dtoMapper), IBookRepository
{
	public async Task<BookDto?> GetByIdAsync(Guid id)
	{
		var book = await DbSet.AsNoTracking().Include(a => a.Author).AsNoTracking().Include(a => a.Categories).AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);
		var bookDto = DtoMapper.Map<BookDto>(book);
		return bookDto;
	}

	public async Task<Book?> AddAsync(BookDto bookDto)
    {
//	    Is this method implements Unit of Work pattern?
        var book = await DbContext.Books.AsNoTracking().FirstOrDefaultAsync(a => a.Title == bookDto.Title);
        if (book is not null)
			return null;	//Results.Conflict("Book already exists");

        var bk = (BookCreateDto)bookDto;
        var author = await DbContext.Authors.FirstOrDefaultAsync(a => a.FirstName == bk.FirstName && a.LastName == bk.LastName);
        if (author == null)
        {
            author = new Author(bk.FirstName, bk.LastName, DateOnly.MinValue);
            DbContext.Authors.Add(author);
        }

        var dbCategories = await DbContext.Categories.AsNoTracking().Where(c => bookDto.Categories.Contains(c.Name)).ToListAsync();
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

		await DbContext.Commit();

		return book;
	}

	public override async Task AddBulkAsync(IEnumerable<Book> entities)
    {
//	    Is this method implements Unit of Work pattern?
	    Dictionary<string, Author> addedAuthors = [];
	    List<Category> existingCategories = [];

	    foreach (Book entity in entities)
	    {
		    var book = await DbContext.Books.AsNoTracking().FirstOrDefaultAsync(a => a.Title == entity.Title);
		    if (book is not null)
			    continue;	//Results.Conflict("Book already exists");

		    var author = await DbContext.Authors.FirstOrDefaultAsync(a => a.FirstName == entity.Author.FirstName && a.LastName == entity.Author.LastName);
		    if (author == null)
		    {
			    var key = entity.Author.FirstName + " " + entity.Author.LastName;
			    if (addedAuthors.ContainsKey(key)) {
				    author = addedAuthors[key];
			    } else {
				    author = DbContext.Authors.Add(entity.Author).Entity;
				    addedAuthors[key] = author;
			    }
		    }

		    entity.Author = author;
		    entity.AuthorId = author.Id;

		    HashSet<string> catNames = entity.Categories.Select(ct => ct.Name).ToHashSet();
		    List<Category> entityCategories = await DbContext.Categories.Where(c => catNames.Contains(c.Name)).ToListAsync();
		    if(entityCategories.Count > 0)
				existingCategories = existingCategories.UnionBy(entityCategories, c => c.Name).ToList();
		    HashSet<string> existingCategoriesSet = existingCategories.Select(c => c.Name).ToHashSet();
		    List<Category> newCategories = entity.Categories.Where(c => !existingCategoriesSet.Contains(c.Name)).ToList();

			if(newCategories.Count > 0)
				await DbContext.Categories.AddRangeAsync(newCategories);

			entityCategories = entityCategories.Concat(newCategories).ToList();
			entity.Categories = entityCategories;

			existingCategories = existingCategories.Concat(newCategories).ToList();

			await DbContext.Books.AddAsync(entity);
	    }

		await DbContext.Commit();
	}

	public async Task UpdateAsync(BookDto dto)
	{
		var book = await DbSet.FindAsync(dto.Id);
		if (book != null)
		{
			book.Title = dto.Title ?? book.Title;
			book.ISBN = dto.ISBN ?? book.ISBN;
			book.Price = dto.Price > 0 ? dto.Price : book.Price;
			book.Year = dto.Year > 0 ? dto.Year : book.Year;
			if (dto.Categories != null)
			{
				book.Categories = DtoMapper.Map<List<Category>>(dto.Categories);
			}
			await DbContext.Commit();
		}
	}

	public override async Task UpdateAsync(Book dto)
	{
		var book = await DbSet.FindAsync(dto.Id);
		if (book != null)
		{
			book.Title = dto.Title ?? book.Title;
			book.ISBN = dto.ISBN ?? book.ISBN;
			book.Price = dto.Price > 0 ? dto.Price : book.Price;
			book.Year = dto.Year > 0 ? dto.Year : book.Year;
			if (dto.Categories != null)
			{
				book.Categories = DtoMapper.Map<List<Category>>(dto.Categories);
			}
			await DbContext.Commit();
		}
	}

	public async Task<StatisticDto> GetStatistics()
	{
		var numBooks = await DbContext.Categories.AsNoTracking().Select(cat => new {name = cat.Name, count = cat.Books.Count}).ToDictionaryAsync(c => c.name, c => c.count);
		var statistics = new StatisticDto{NumBooksByCategory = numBooks};
		var avgPrices = await DbContext.Authors.AsNoTracking().Include(a => a.Books)
			.Select(p => new {name = p.FirstName + " " + p.LastName, avg = p.Books.Count > 0 ? p.Books.Select(b => b.Price).Average() : 0}).ToDictionaryAsync(c => c.name, c => c.avg);
		statistics.AvgPriceByAuthor = avgPrices;
		var top = await DbSet.AsNoTracking().Take(10).OrderByDescending(b => b.Price).ToDictionaryAsync(b => b.Title, b => b.Price);
		statistics.TopPrices = top;

		return statistics;
	}

	public async Task<List<Book>> Search(int? page, int? pageSize, string? title, string? author, string? category, decimal? minPrice, decimal? maxPrice)
	{
		IQueryable<Book> query = DbSet.AsNoTracking().Include(b => b.Author).AsNoTracking().Include(b => b.Categories).AsNoTracking();
		if(title is not null)
			query = query.Where(b => b.Title.Contains(title));
		if(minPrice is not null)
			query = query.Where(b => b.Price >= minPrice).OrderBy(b => b.Price);
		if(maxPrice is not null)
			query = query.Where(b => b.Price <= maxPrice).OrderByDescending(b => b.Price);
		if(author is not null)
			query = query.Where(b => b.Author.FirstName.Contains(author) || b.Author.LastName.Contains(author));
		if(category is not null)
			query = query.Where(b => b.Categories.Select(c => c.Name).Any(cat => cat.Contains(category)));
		if (page > 0)
			query = query.Skip((page - 1) * pageSize ?? 0).Take(pageSize ?? 10);
		var res = await query.ToListAsync();
		return res;
	}
}