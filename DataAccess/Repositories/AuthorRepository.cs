using AutoMapper;
using DataAccess.Repositories.Impl;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class AuthorRepository(BookStoreDbContext dbContext, IMapper dtoMapper) : EfRepository<Author, AuthorDto>(dbContext, dtoMapper), IAuthorRepository
{
	public override async Task<AuthorDto?> GetByIdAsync(Guid id)
	{
		var a = await DbSet.Include(a => a.Books)
			.ThenInclude(b => b.Categories)
			.SingleOrDefaultAsync(a => a.Id == id);
		var dto = DtoMapper.Map<AuthorDto>(a);
		return dto;
	}

	public async Task<AuthorDto?> GetByIdWithBooksAsync(Guid id)
	{
		var a = await DbSet.Include(a => a.Books)
			.ThenInclude(b => b.Categories)
			.Select(a => new AuthorDto{Id = a.Id, FirstName = a.FirstName, LastName = a.LastName, BooksRaw = DtoMapper.Map<List<BookDto>>(a.Books), Biography = a.Biography, BirthDate = a.BirthDate})
			.SingleOrDefaultAsync(a => a.Id == id);
		return a;
	}

	public override async Task UpdateAsync(AuthorDto dto)
	{
		var author = await DbSet.FindAsync(dto.Id);
		if (author != null)
		{
			author.FirstName = dto.FirstName ?? author.FirstName;
			author.LastName = dto.LastName ?? author.LastName;
			author.BirthDate = dto.BirthDate != null ? dto.BirthDate : author.BirthDate;
			author.Biography = dto.Biography ?? author.Biography;
			await DbContext.SaveChangesAsync();
		}
	}
}