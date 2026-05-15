using Application.Interfaces;
using AutoMapper;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BooksAPI.IntegrationTests.Configuration;

public class BaseTestClient : IDisposable
{
	private readonly IServiceScope _scope;
	private readonly CustomWebApplicationFactory _factory;
	protected readonly BookStoreDbContext DbContext;
	protected readonly HttpClient TestHttpClient;
//	protected readonly ISender Sender;
//	protected readonly IMapper Mapper;
//	protected readonly IMediator Mediator;
//	protected readonly IBookRepository BookRepository;

	protected BaseTestClient(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_scope = factory.Services.CreateScope();
		TestHttpClient = factory.CreateAuthenticatedClient();
		DbContext = _scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
	}

	public void Dispose()
	{
		_scope?.Dispose();
		DbContext?.Dispose();
	}
}