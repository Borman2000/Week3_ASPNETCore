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
using MySqlConnector;

namespace BooksAPI.IntegrationTests.Configuration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestConnectionString =
        "Server=localhost;Port=33006;Database=BooksDBTests;User Id=root;Password=root;";
    private const string TestDatabaseName = "BooksDBTests";
    private const string TestJwtSecret = "BooksApiIntegrationTestsSecretKey12345";

    public CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", TestJwtSecret);
    }

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
        return CreateAuthenticatedClient(CreateDefaultClaims());
    }

    public HttpClient CreateAuthenticatedClient(IEnumerable<Claim> claims)
    {
        var tokenGenerator = Services.GetRequiredService<ITokenGenerator>();

        var token = tokenGenerator.GenerateJwtToken(("test-user-id", "test@user.com", claims.ToList()));

        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static List<Claim> CreateDefaultClaims()
    {
        return
        [
            new Claim(ClaimType.Books.Read, "true"),
            new Claim(ClaimType.Books.Create, "true"),
            new Claim(ClaimType.Books.Update, "true"),
            new Claim(ClaimType.Authors.Read, "true"),
            new Claim(ClaimType.Authors.Create, "true"),
            new Claim(ClaimType.Categories.Create, "true"),
            new Claim(ClaimType.Categories.Read, "true"),
        ];
    }

    public async Task InitializeAsync()
    {
        var connectionBuilder = new MySqlConnectionStringBuilder(TestConnectionString) { Database = string.Empty };

        await using (var connection = new MySqlConnection(connectionBuilder.ConnectionString))
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = $"DROP DATABASE IF EXISTS `{TestDatabaseName}`;";
            await command.ExecuteNonQueryAsync();
        }

        _ = CreateClient();
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
