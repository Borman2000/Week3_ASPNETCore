using Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MySql;

namespace IntegrationTests.Configuration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _dbContainer = new MySqlBuilder()
		    .WithImage("mysql:8.0")
		    .WithDatabase("BooksDBTests")
		    .WithPortBinding(65530, 3306)
		    .WithUsername("root")
		    .WithPassword("root")
		    .Build();

//    private Respawner _respawner = null!;

    public HttpClient HttpClient { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
	    builder.ConfigureServices(services =>
	    {
//		    var descriptorType = typeof(DbContextOptions<BookStoreDbContext>);
//		    var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);
//		    if (descriptor is not null) services.Remove(descriptor);

		    services.RemoveAll<DbContextOptions<BookStoreDbContext>>();

		    services.AddDbContext<BookStoreDbContext>(options =>
		    {
			    var connectionString = _dbContainer.GetConnectionString();
			    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
		    });
	    });
    }

    public async Task InitializeAsync()
    {
	    await _dbContainer.StartAsync();

	    using var scope = Services.CreateScope();
	    var dbContext = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
	    await dbContext.Database.MigrateAsync();

//	    await InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
//        await _dbContainer.DisposeAsync();
        await _dbContainer.StopAsync();
    }

//    public async Task ResetDatabaseAsync()
//    {
//	    await _respawner.ResetAsync(_dbConnection);
//    }
//
//    private async Task InitializeRespawnerAsync()
//    {
//	    _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
//	    {
//		    SchemasToInclude = [ "shipping" ],
//		    DbAdapter = DbAdapter.Postgres
//	    });
//    }
}
