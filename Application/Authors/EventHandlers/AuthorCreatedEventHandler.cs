using Application.Interfaces;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Authors.EventHandlers;

public class AuthorCreatedEventHandler : INotificationHandler<AuthorCreatedEvent>
{
	private readonly IEmailService _emailService;
	private readonly ILogger _logger;

	public AuthorCreatedEventHandler(IEmailService emailService, ILogger<AuthorCreatedEventHandler> logger)
	{
		_emailService = emailService;
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task Handle(AuthorCreatedEvent domainEvent, CancellationToken cancellationToken)
	{
		_logger.LogInformation($"Added author: {domainEvent.FirstName}  {domainEvent.LastName}");
	}
}