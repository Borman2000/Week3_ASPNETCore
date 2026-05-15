using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Application.Books.Create;
using Application.DTOs;
using BooksAPI.IntegrationTests.Configuration;
using BooksAPI.IntegrationTests.Helpers;
using Common.JwtHelperService.Models;

namespace BooksAPI.IntegrationTests;

[Collection("ApiTests")]
public class AuthorizationApiTests(CustomWebApplicationFactory factory)
{
    [Fact]
    public async Task CreateBook_Without_Token_Should_Return_Unauthorized()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = new CreateBookCommand($"Unauthorized-{Guid.NewGuid():N}", TestValues.ISBN_VALID, TestValues.AUTHOR_ID_EXISTS, 9.99m);

        // Act
        var response = await client.PostAsJsonAsync("/api/v2/books", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_Without_Token_Should_Return_Unauthorized()
    {
        // Arrange
        using var client = factory.CreateClient();
        var request = new CategoryDto { Name = $"Unauthorized-{Guid.NewGuid():N}", Description = "Unauthorized request" };

        // Act
        var response = await client.PostAsJsonAsync("/api/v2/categories", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_With_Insufficient_Claims_Should_Return_Forbidden()
    {
        // Arrange
        using var client = factory.CreateAuthenticatedClient(CreateReadOnlyClaims());
        var request = new CreateBookCommand($"Forbidden-{Guid.NewGuid():N}", TestValues.ISBN_VALID, TestValues.AUTHOR_ID_EXISTS, 9.99m);

        // Act
        var response = await client.PostAsJsonAsync("/api/v2/books", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_With_Insufficient_Claims_Should_Return_Forbidden()
    {
        // Arrange
        using var client = factory.CreateAuthenticatedClient(CreateReadOnlyClaims());
        var request = new CategoryDto { Name = $"Forbidden-{Guid.NewGuid():N}", Description = "Forbidden request" };

        // Act
        var response = await client.PostAsJsonAsync("/api/v2/categories", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static List<Claim> CreateReadOnlyClaims()
    {
        return
        [
            new Claim(ClaimType.Books.Read, "true"),
            new Claim(ClaimType.Authors.Read, "true"),
            new Claim(ClaimType.Categories.Read, "true")
        ];
    }
}

