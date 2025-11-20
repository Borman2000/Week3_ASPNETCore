using MediatR;

namespace Domain.Events;

public record PriceChangedEvent(Guid BookId, decimal OldPrice, decimal NewPrice) : INotification;