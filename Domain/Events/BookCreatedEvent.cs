using MediatR;

namespace Domain.Events;

public record BookCreatedEvent(Guid BookId, string Title, Guid AuthorId) : INotification;