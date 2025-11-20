using Application.Interfaces;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Books.EventHandlers;

public class BookCreatedEventHandler : INotificationHandler<BookCreatedEvent>
{
	private readonly IEmailService _emailService;
	private readonly ILogger _logger;

	public BookCreatedEventHandler(IEmailService emailService, ILogger<BookCreatedEventHandler> logger)
	{
		_emailService = emailService;
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task Handle(BookCreatedEvent domainEvent, CancellationToken cancellationToken)
	{
		await _emailService.SendBookCreatedEmail(domainEvent.Title, domainEvent.AuthorId);
		_logger.LogInformation($"Added book \"{domainEvent.Title}\"");
	}
}