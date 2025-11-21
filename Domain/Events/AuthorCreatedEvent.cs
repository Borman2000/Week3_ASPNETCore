using MediatR;

namespace Domain.Events;

public record AuthorCreatedEvent(Guid AuthorId, string FirstName, string LastName) : INotification;