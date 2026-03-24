using AuthAPI.Application.DTOs;
using AuthAPI.Application.Interfaces;
using AuthAPI.Domain;
using AuthAPI.Domain.Entities;
using AuthAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Api;

public static class Endpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

        app.MapPost("/users/login", (IIdentityService identityService, [FromQuery] string login, [FromQuery] string psw) => identityService.SigninUserAsync(login, psw)).WithDescription("admin@email.com / Test1234!");
//        app.MapPost("/users/login", (IUserRepository<ApplicationUser, UserDto> usersRepoService, [FromQuery] string login, [FromQuery] string psw) => usersRepoService.Login(login, psw)).ExcludeFromDescription();
        app.MapGet("/users", (IUserRepository<ApplicationUser, UserDto> usersRepoService) => usersRepoService.GetAllAsync()).RequireAuthorization(ClaimType.Users.Read);
        app.MapGet("/users/{id:guid}", (IUserRepository<ApplicationUser, UserDto> usersRepoService, [FromRoute] Guid id) =>  usersRepoService.GetByIdAsync(id)).RequireAuthorization(ClaimType.Users.Read);
        app.MapPost("/users", (IUserRepository<ApplicationUser, UserDto> usersRepoService, UserDto user) => usersRepoService.AddAsync(user.UserName, user.Password, user.Roles)).RequireAuthorization(ClaimType.Users.Create);
        app.MapPut("/users/", (IUserRepository<ApplicationUser, UserDto> usersRepoService, ApplicationUser user) => usersRepoService.UpdateAsync(user)).RequireAuthorization(ClaimType.Users.Update);
        app.MapDelete("/users/{id:guid}", (IUserRepository<ApplicationUser, UserDto> usersRepoService, [FromRoute] Guid id) => usersRepoService.DeleteAsync(id)).RequireAuthorization(ClaimType.Users.Delete);
    }
}