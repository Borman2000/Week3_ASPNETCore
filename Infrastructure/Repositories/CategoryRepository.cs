using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class CategoryRepository(BookStoreDbContext dbContext, IMapper dtoMapper)
	: EfRepository<Category>(dbContext, dtoMapper), ICategoryRepository
{

	public override async Task UpdateAsync(Category dto)
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