using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories;

public static class BooksSeedService
{
	public static async Task SeedAsync(IServiceProvider serviceProvider)
	{
		var dbContext = serviceProvider.GetRequiredService<BookStoreDbContext>();

		await dbContext.Database.MigrateAsync();

		if (await dbContext.Books.AnyAsync())
		{
			return;
		}

// seed any data here if OnModelCreating doesn't have required ones

		await dbContext.SaveChangesAsync();
	}
}
