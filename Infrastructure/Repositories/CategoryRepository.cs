using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Repositories.Impl;

namespace Infrastructure.Repositories;

public class CategoryRepository(BookStoreDbContext dbContext, IMapper dtoMapper)
	: EfRepository<Category, CategoryDto>(dbContext, dtoMapper), ICategoryRepository
{

	public override async Task UpdateAsync(CategoryDto dto)
	{
		var category = await DbSet.FindAsync(dto.Id);
		if (category != null)
		{
			category.Name = dto.Name ?? category.Name;
			category.Description = dto.Description ?? category.Description;
			await DbContext.SaveChangesAsync();
		}
	}
}