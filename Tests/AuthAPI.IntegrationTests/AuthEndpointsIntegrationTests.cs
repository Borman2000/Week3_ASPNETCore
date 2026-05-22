using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AuthAPI.Domain.Entities;
using AuthAPI.IntegrationTests.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AuthAPI.IntegrationTests;

[Collection("AuthApiIntegrationTests")]
public class AuthEndpointsIntegrationTests(AuthApiWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Login_With_Valid_Admin_Credentials_Returns_Token()
    {
        var response = await _client.PostAsJsonAsync("/users/login", new { Login = "admin@email.com", Password = "Test1234!" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var token = await ReadTokenAsync(response);
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Login_With_Invalid_Password_Returns_Unauthorized()
    {
        var response = await _client.PostAsJsonAsync("/users/login", new { Login = "admin@email.com", Password = "WrongPassword1!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_With_Unknown_User_Returns_NotFound()
    {
        var response = await _client.PostAsJsonAsync("/users/login", new { Login = "unknown_user@email.com", Password = "Test1234!" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Users_Without_Token_Returns_Unauthorized()
    {
        var response = await _client.GetAsync("/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Users_With_Admin_Token_Returns_Ok()
    {
        var token = await LoginAsAdminAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Users_With_Non_Admin_User_Returns_Forbidden()
    {
        var userEmail = $"user_{Guid.NewGuid():N}@email.com";

        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser
            {
                Email = userEmail,
                UserName = userEmail
            };

            var createResult = await userManager.CreateAsync(user, "Test1234!");
            Assert.True(createResult.Succeeded);

            var addRoleResult = await userManager.AddToRoleAsync(user, "User");
            Assert.True(addRoleResult.Succeeded);
        }

        var loginResponse = await _client.PostAsJsonAsync("/users/login", new { Login = userEmail, Password = "Test1234!" });
        var userToken = await ReadTokenAsync(loginResponse);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_User_With_Admin_Token_Returns_Created()
    {
        var token = await LoginAsAdminAsync();
        var newUserEmail = $"create_{Guid.NewGuid():N}@email.com";

        using var request = new HttpRequestMessage(HttpMethod.Post, "/users")
        {
            Content = JsonContent.Create(new
            {
                UserName = newUserEmail,
                Password = "Test1234!",
                Roles = new[] { "User" },
                Email = newUserEmail
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_User_Without_Token_Returns_Unauthorized()
    {
        var newUserEmail = $"unauth_{Guid.NewGuid():N}@email.com";

        var response = await _client.PostAsJsonAsync("/users", new
        {
            UserName = newUserEmail,
            Password = "Test1234!",
            Roles = new[] { "User" },
            Email = newUserEmail
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<string> LoginAsAdminAsync()
    {
        var response = await _client.PostAsJsonAsync("/users/login", new { Login = "admin@email.com", Password = "Test1234!" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return await ReadTokenAsync(response);
    }

    private static async Task<string> ReadTokenAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("token", out var camel))
        {
            return camel.GetString()!;
        }

        if (doc.RootElement.TryGetProperty("Token", out var pascal))
        {
            return pascal.GetString()!;
        }

        throw new InvalidOperationException($"Token field not found in response: {json}");
    }
}

