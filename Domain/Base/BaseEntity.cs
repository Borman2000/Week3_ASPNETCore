using AutoMapper.Configuration.Annotations;
using MediatR;

namespace Domain.Base;

public abstract class BaseEntity
{
    public Guid Id {get; init;} = Guid.Empty;

    private List<INotification> _domainEvents;
    [Ignore]
    public List<INotification> DomainEvents => _domainEvents;

    public void AddDomainEvent(INotification eventItem)
    {
	    _domainEvents = _domainEvents ?? new List<INotification>();
	    _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
	    _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
	    _domainEvents?.Clear();
    }
}