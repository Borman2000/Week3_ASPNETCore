using System.Net.Http.Headers;
using System.Security.Claims;
using Common.JwtHelperService.Interfaces;
using Common.JwtHelperService.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BooksAPI.IntegrationTests.Configuration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestConnectionString =
        "Server=localhost;Port=33006;Database=BooksDBTests;User Id=root;Password=root;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BookStoreDbContext>>();

            services.AddDbContext<BookStoreDbContext>(options =>
            {
                options.UseMySql(TestConnectionString, ServerVersion.AutoDetect(TestConnectionString));
            });
        });
    }

    public HttpClient CreateAuthenticatedClient()
    {
        var tokenGenerator = Services.GetRequiredService<ITokenGenerator>();

        var claims = new List<Claim>
        {
            new(ClaimType.Books.Read, "true"),
            new(ClaimType.Books.Create, "true"),
            new(ClaimType.Books.Update, "true"),
            new(ClaimType.Authors.Read, "true"),
            new(ClaimType.Authors.Create, "true"),
            new(ClaimType.Categories.Create, "true"),
            new(ClaimType.Categories.Read, "true"),
        };

        var token = tokenGenerator.GenerateJwtToken(("test-user-id", "test@user.com", claims));

        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
