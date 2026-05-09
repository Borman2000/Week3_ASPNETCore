using AuthAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MySql;

namespace AuthAPI.IntegrationTests.Configuration;

public class AuthApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _dbContainer = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .WithDatabase("UsersDbTests")
        .WithUsername("root")
        .WithPassword("root")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<UsersDbContext>>();

            services.AddDbContext<UsersDbContext>(options =>
            {
                var connectionString = _dbContainer.GetConnectionString();
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Trigger host startup so Program.SeedAsync runs against test container.
        _ = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
