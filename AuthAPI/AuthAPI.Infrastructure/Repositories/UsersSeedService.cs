using System.Security.Claims;
using AuthAPI.Domain;
using AuthAPI.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthAPI.Infrastructure.Repositories;

public static class UsersSeedService
{
	public static async Task SeedAsync(IServiceProvider serviceProvider)
	{
		var dbContext = serviceProvider.GetRequiredService<UsersDbContext>();
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		await dbContext.Database.MigrateAsync();

		if (await dbContext.Users.AnyAsync())
		{
			return;
		}

		var adminRole = new IdentityRole { Name = nameof(RoleEnum.Admin) };
		var creatorRole = new IdentityRole { Name = nameof(RoleEnum.Creator) };
		var editorRole = new IdentityRole { Name = nameof(RoleEnum.Maintainer) };
		var userRole = new IdentityRole { Name = nameof(RoleEnum.User) };

		var result = await roleManager.CreateAsync(adminRole);
		result = await roleManager.CreateAsync(creatorRole);
		result = await roleManager.CreateAsync(editorRole);
		result = await roleManager.CreateAsync(userRole);

		result = await roleManager.AddClaimAsync(adminRole, new Claim(ClaimType.Users.Read, "true"));
		result = await roleManager.AddClaimAsync(adminRole, new Claim(ClaimType.Users.Create, "true"));
		result = await roleManager.AddClaimAsync(adminRole, new Claim(ClaimType.Users.Update, "true"));
		result = await roleManager.AddClaimAsync(adminRole, new Claim(ClaimType.Users.Delete, "true"));

		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Books.Read, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Books.Create, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Books.Update, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Books.Delete, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Authors.Read, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Authors.Create, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Authors.Update, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Authors.Delete, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Categories.Read, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Categories.Create, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Categories.Update, "true"));
		result = await roleManager.AddClaimAsync(creatorRole, new Claim(ClaimType.Categories.Delete, "true"));

		result = await roleManager.AddClaimAsync(editorRole, new Claim(ClaimType.Authors.Read, "true"));
		result = await roleManager.AddClaimAsync(editorRole, new Claim(ClaimType.Authors.Update, "true"));
		result = await roleManager.AddClaimAsync(editorRole, new Claim(ClaimType.Books.Read, "true"));
		result = await roleManager.AddClaimAsync(editorRole, new Claim(ClaimType.Books.Update, "true"));
		result = await roleManager.AddClaimAsync(editorRole, new Claim(ClaimType.Categories.Read, "true"));
		result = await roleManager.AddClaimAsync(editorRole, new Claim(ClaimType.Categories.Update, "true"));

		result = await roleManager.AddClaimAsync(userRole, new Claim(ClaimType.Authors.Read, "true"));
		result = await roleManager.AddClaimAsync(userRole, new Claim(ClaimType.Books.Read, "true"));
		result = await roleManager.AddClaimAsync(userRole, new Claim(ClaimType.Categories.Read, "true"));

		var adminUser = new ApplicationUser
		{
			Id = Guid.NewGuid().ToString(),
			Email = "admin@email.com",
			UserName = "admin@email.com"
		};

		result = await userManager.CreateAsync(adminUser, "Test1234!");
		result = await userManager.AddToRoleAsync(adminUser, nameof(RoleEnum.Admin));

//		var authorUser = new ApplicationUser
//		{
//			Id = Guid.NewGuid().ToString(),
//			Email = "author@test.com",
//			UserName = "author@test.com"
//		};
//
//		result = await userManager.CreateAsync(authorUser, "Test1234!");
//		result = await userManager.AddToRoleAsync(authorUser, "Author");

		await dbContext.SaveChangesAsync();
	}
}
