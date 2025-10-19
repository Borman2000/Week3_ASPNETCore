using AutoMapper;
using DataAccess.Repositories.Impl;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class AuthorRepository(BookStoreDbContext dbContext) : EfRepository<Author, AuthorDto>(dbContext), IAuthorRepository
{
	public new async Task<IEnumerable<Author>> GetAllAsync()
	{
		return await DbSet.Include(entity => entity.Books).ToListAsync();
	}

	public async Task<AuthorDto?> GetByIdAsync(Guid id, IMapper mapper)
	{
		var a = await DbSet.Include(a => a.Books)
			.ThenInclude(b => b.Categories)
			.SingleOrDefaultAsync(a => a.Id == id);
		var dto = mapper.Map<AuthorDto>(a);
		return dto;
	}
}