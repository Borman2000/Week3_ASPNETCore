using AuthAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MySqlConnector;

namespace AuthAPI.IntegrationTests.Configuration;

public class AuthApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestConnectionString =
        "Server=localhost;Port=33006;Database=UsersDbTests;User Id=root;Password=root;";
    private const string TestDatabaseName = "UsersDbTests";

    public AuthApiWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<UsersDbContext>>();

            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseMySql(TestConnectionString, ServerVersion.AutoDetect(TestConnectionString));
            });
        });
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
