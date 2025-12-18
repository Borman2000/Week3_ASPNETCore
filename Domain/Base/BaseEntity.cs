using AutoMapper.Configuration.Annotations;
using MediatR;

namespace Domain.Base;

public abstract class BaseEntity
{
    public Guid Id {get; init;} = Guid.Empty;

    [Ignore]
    public List<INotification>? DomainEvents { get; set; }

    public void AddDomainEvent(INotification eventItem)
    {
	    DomainEvents ??= [];
	    DomainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
	    DomainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
	    DomainEvents?.Clear();
    }
}