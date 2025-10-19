namespace Domain.Base;

public abstract class BaseEntity
{
    public Guid Id {get; init;} = Guid.Empty;
}