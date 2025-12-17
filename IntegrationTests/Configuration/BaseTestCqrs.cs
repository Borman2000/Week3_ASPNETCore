using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Configuration;

public class BaseTestCqrs : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
	private readonly IServiceScope _scope;
	protected readonly HttpClient TestHttpClient;
	protected readonly BookStoreDbContext DbContext;

	protected BaseTestCqrs(CustomWebApplicationFactory factory)
	{
		_scope = factory.Services.CreateScope();

		TestHttpClient = factory.CreateClient();

		DbContext = _scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
	}


	public void Dispose()
	{
		_scope?.Dispose();
		DbContext?.Dispose();
	}
}