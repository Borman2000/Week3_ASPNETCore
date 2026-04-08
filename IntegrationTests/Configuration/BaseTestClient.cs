using Application.Interfaces;
using AutoMapper;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Configuration;

public class BaseTestClient : IClassFixture<CustomWebApplicationFactory>, IDisposable
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
		TestHttpClient = _factory.CreateClient();
		DbContext = _scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
//		Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
//		BookRepository = _scope.ServiceProvider.GetRequiredService<IBookRepository>();
//		Mapper = factory.Services.GetRequiredService<IMapper>();
//		Mediator = factory.Services.GetRequiredService<IMediator>();
	}

	public void Dispose()
	{
		_scope?.Dispose();
		DbContext?.Dispose();
	}
}