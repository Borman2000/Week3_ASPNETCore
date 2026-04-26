using Common.JwtHelperService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationAPI.Domain;
using NotificationAPI.Domain.Interfaces;
using NotificationAPI.Infrastructure.Services;

namespace NotificationAPI.Api;

public static class Endpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

        app.MapPost("/notify", (NotificationDispatcher dispatcher, NotificationRequest request) => dispatcher.DispatchAsync(request));
        app.MapGet("/notify/{id:guid}", (INotificationRepository repository, [FromRoute] Guid id) =>  repository.GetResultsAsync(id)).RequireAuthorization(ClaimType.Users.Read);
    }
}