using AuthAPI.Application.DTOs;
using AuthAPI.Domain;
using AuthAPI.Domain.Entities;
using AuthAPI.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Infrastructure.Repositories;

public class UserRepository : IUserRepository<ApplicationUser, UserDto>
{
	protected readonly UsersDbContext DbContext;
	protected readonly DbSet<ApplicationUser> DbSet;
	protected readonly IMapper DtoMapper;
	protected readonly UserManager<ApplicationUser> UserManager;
	protected readonly SignInManager<ApplicationUser> SignInManager;
	protected readonly RoleManager<IdentityRole> RoleManager;

	public UserRepository(UsersDbContext context, IMapper dtoMapper, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
	{
		DbContext = context;
		DbSet = context.Set<ApplicationUser>();
		DtoMapper = dtoMapper;
		UserManager = userManager;
		SignInManager = signInManager;
		RoleManager = roleManager;
	}

	public virtual async Task<IResult> AddAsync(string login, string password, IList<string>? roles)
	{
		var user = new ApplicationUser
		{
			Id = Guid.NewGuid().ToString(),
			Email = login,
			UserName = login
		};
		var result = await UserManager.CreateAsync(user, password);
		if (!result.Succeeded)
		{
			return Results.BadRequest(result.Errors);
		}

		if (roles == null || roles.Count == 0)
		{
			var roleResult = await UserManager.AddToRoleAsync(user, nameof(RoleEnum.User));
			if (!roleResult.Succeeded)
			{
				return Results.BadRequest(roleResult.Errors);
			}
		}
		else
		{
			foreach (string role in roles)
			{
				var roleResult = await UserManager.AddToRoleAsync(user, role);
				if (!roleResult.Succeeded)
				{
					return Results.BadRequest(roleResult.Errors);
				}
			}
		}


		return Results.Created($"{user.Id}", user.Id);
	}

	public virtual async Task<IResult> UpdateAsync(ApplicationUser entity)
	{
		DbContext.Entry(entity).State = EntityState.Modified;
		await DbContext.SaveChangesAsync();
		return Results.Ok();
	}

	public async Task<IResult> DeleteAsync(Guid id)
	{
		var user = await DbContext.Set<ApplicationUser>().FindAsync(id.ToString());
		if(user == null)
			return Results.NotFound();

		DbContext.Set<ApplicationUser>().Remove(user);
		await DbContext.SaveChangesAsync();
		return Results.Ok();
	}

	public async Task<ApplicationUser?> GetByIdAsync(Guid id)
	{
		return await DbContext.Set<ApplicationUser>().FindAsync(id.ToString());
	}

	public async Task<IResult> Login(string login, string password)
	{
		throw new NotImplementedException();
	}

	public async Task<IList<UserDto>> GetAllAsync()
	{
		var users = await DbSet.AsNoTracking().ToListAsync();
		return DtoMapper.Map<List<UserDto>>(users);
	}
}