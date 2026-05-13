using AuthAPI.Application.DTOs;
using AuthAPI.Application.Interfaces;
using AuthAPI.Domain;
using AuthAPI.Domain.Entities;
using AutoMapper;
using Common.JwtHelperService.Interfaces;
using Common.OpenTelemetryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Infrastructure.Services;

public class IdentityService : IIdentityService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IMapper _mapper;
	private readonly ITokenGenerator _tokenGenerator;

	public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IMapper dtoMapper, ITokenGenerator tokenGenerator)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_roleManager = roleManager;
		_roleManager = roleManager;
		_mapper = dtoMapper;
		_tokenGenerator = tokenGenerator;
	}

	public async Task<IResult> AssignUserToRole(string userName, IList<string> roles)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
		if (user == null)
		{
			return Results.NotFound("User not found");
		}

		var result = await _userManager.AddToRolesAsync(user, roles);
		return Results.Ok();
	}

	public async Task<IResult> CreateRoleAsync(string roleName)
	{
		var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
		if (!result.Succeeded)
		{
			return Results.ValidationProblem(result.Errors.ToDictionary<IdentityError, string, string[]>(error => error.Code, error => [error.Description]));
		}
		return Results.Ok();
	}


	// Return multiple value
	public async Task<IResult> CreateUserAsync(string userName, string password, string email, string fullName, List<string> roles)
	{
		var user = new ApplicationUser()
		{
			FullName = fullName,
			UserName = userName,
			Email = email
		};

		var result = await _userManager.CreateAsync(user, password);

		if (!result.Succeeded)
		{
			return Results.ValidationProblem(result.Errors.ToDictionary<IdentityError, string, string[]>(error => error.Code, error => [error.Description]));
		}

		var addUserRole = await _userManager.AddToRolesAsync(user, roles);
		if (!addUserRole.Succeeded)
		{
			return Results.ValidationProblem(addUserRole.Errors.ToDictionary<IdentityError, string, string[]>(error => error.Code, error => [error.Description]));
		}
		return Results.Ok(user.Id);
	}

	public async Task<IResult> DeleteRoleAsync(string roleId)
	{
		var roleDetails = await _roleManager.FindByIdAsync(roleId);
		if (roleDetails == null)
		{
			return Results.NotFound("Role not found");
		}

		if (roleDetails.Name == nameof(RoleEnum.Admin))
		{
			return Results.BadRequest("You can not delete Administrator Role");
		}
		var result = await _roleManager.DeleteAsync(roleDetails);
		if (!result.Succeeded)
		{
			return Results.ValidationProblem(result.Errors.ToDictionary<IdentityError, string, string[]>(error => error.Code, error => [error.Description]));
		}
		return Results.Ok();
	}

	public async Task<IResult> DeleteUserAsync(string userId)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
		if (user == null)
		{
			return Results.NotFound("User not found");
		}

		if (user.UserName == "system" || user.UserName == "admin")
		{
			return Results.Problem("You can not delete system or admin user");
			//throw new BadRequestException("You can not delete system or admin user");
		}
		var result = await _userManager.DeleteAsync(user);
		return Results.Ok(result);
	}

	public async Task<List<(string id, string fullName, string userName, string email)>> GetAllUsersAsync()
	{
		var users = await _userManager.Users.Select(x => new
		{
			x.Id,
			x.FullName,
			x.UserName,
			x.Email
		}).ToListAsync();

		return users.Select(user => (user.Id, user.FullName, user.UserName, user.Email)).ToList();
	}

	public Task<List<(string id, string userName, string email, IList<string> roles)>> GetAllUsersDetailsAsync()
	{
		throw new NotImplementedException();

		//var roles = await _userManager.GetRolesAsync(user);
		//return (user.Id, user.UserName, user.Email, roles);

		//var users = _userManager.Users.ToListAsync();
	}

	public async Task<List<(string id, string roleName)>> GetRolesAsync()
	{
		var roles = await _roleManager.Roles.Select(x => new
		{
			x.Id,
			x.Name
		}).AsNoTracking().ToListAsync();

		return roles.Select(role => (role.Id, role.Name)).ToList();
	}

	public async Task<IResult>GetUserDetailsAsync(string userId)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
		if (user == null)
		{
			return Results.NotFound("User not found");
		}
		UserDto result = _mapper.Map<UserDto>(user);
		result.Roles = await _userManager.GetRolesAsync(user);
		return Results.Ok(result);
	}

	public async Task<IResult> GetUserDetailsByUserNameAsync(string userName)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
		if (user == null)
		{
			return Results.NotFound("User not found");
		}
		UserDto result = _mapper.Map<UserDto>(user);
		result.Roles = await _userManager.GetRolesAsync(user);
		return Results.Ok(result);
	}

	public async Task<IResult> GetUserIdAsync(string userName)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
		if (user == null)
		{
			return Results.NotFound("User not found");
			//throw new Exception("User not found");
		}

		return Results.Ok(await _userManager.GetUserIdAsync(user));
	}

	public async Task<IResult> GetUserNameAsync(string userId)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
		if (user == null)
		{
			return Results.NotFound("User not found");
			//throw new Exception("User not found");
		}
		return Results.Ok(await _userManager.GetUserNameAsync(user));
	}

	public async Task<IResult> GetUserRolesAsync(string userId)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
		if (user == null)
		{
			return Results.NotFound("User not found");
		}
		var roles = await _userManager.GetRolesAsync(user);
		return Results.Ok(roles.ToList());
	}

	public async Task<IResult> IsInRoleAsync(string userId, string role)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

		if (user == null)
		{
			return Results.NotFound("User not found");
		}
		return Results.Ok(await _userManager.IsInRoleAsync(user, role));
	}

	public async Task<IResult> IsUniqueUserName(string userName)
	{
		return Results.Ok(await _userManager.FindByNameAsync(userName) == null);
	}

	public async Task<IResult> SigninUserAsync(string userName, string password)
	{
		using var activity = ActivitySourceProvider.Source?.StartActivity();
		activity?.AddEvent(new("User login: started."));
		activity?.SetTag("username", userName);

		var user = await _userManager.FindByEmailAsync(userName);
		if (user is null)
		{
			activity?.AddEvent(new("User login: user not found."));
			return Results.NotFound("User not found");
		}

		var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
		if (!result.Succeeded)
		{
			activity?.AddEvent(new("User login: invalid credentials."));
			return Results.Unauthorized();
		}

		var roles = await _userManager.GetRolesAsync(user);
		var userRole = roles.FirstOrDefault() ?? nameof(RoleEnum.User);

		var role = await _roleManager.FindByNameAsync(userRole);
		var roleClaims = role is not null ? await _roleManager.GetClaimsAsync(role) : [];
//		var roleClaims = role is not null ? await _userManager.GetRolesAsync(user) : [];

		var userIdentifier = user.UserName ?? user.Email;
		if (string.IsNullOrWhiteSpace(userIdentifier))
		{
			activity?.AddEvent(new("User login: user identity is invalid."));
			return Results.Problem("User identity is invalid.");
		}

		var token = _tokenGenerator.GenerateJwtToken((user.Id, userIdentifier, roleClaims));
		activity?.AddEvent(new("User login: success."));
		return Results.Ok(new { Token = token });
	}

	public async Task<IResult> UpdateUserProfile(string id, string fullName, string email, IList<string> roles)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user == null)
		{
			return Results.NotFound("User not found");
		}
		user.FullName = fullName;
		user.Email = email;
		var result = await _userManager.UpdateAsync(user);

		return Results.Ok(result);
	}

	public async Task<IResult> GetRoleByIdAsync(string id)
	{
		var role = await _roleManager.FindByIdAsync(id);
		if (role == null)
		{
			return Results.NotFound("Role not found");
		}
		return Results.Ok(role.Name);
	}

	public async Task<IResult> UpdateRole(string id, string roleName)
	{
		if (!String.IsNullOrEmpty(roleName))
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null)
			{
				return Results.NotFound("Role not found");
			}
			role.Name = roleName;
			var result = await _roleManager.UpdateAsync(role);
			return Results.Ok(result);
		}

		return Results.BadRequest(roleName);
	}

	public async Task<IResult> UpdateUsersRole(string userName, IList<string> usersRole)
	{
		var user = await _userManager.FindByNameAsync(userName);
		if (user == null)
		{
			return Results.NotFound("User not found");
		}
		var existingRoles = await _userManager.GetRolesAsync(user);
		var result = await _userManager.RemoveFromRolesAsync(user, existingRoles);
		result = await _userManager.AddToRolesAsync(user, usersRole);

		return Results.Ok(result);
	}
}
