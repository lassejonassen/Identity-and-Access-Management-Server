namespace Abstractions;

public abstract class Entity
{
    protected Entity(Guid id, Guid? correlationId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("The entity identifier must not be empty", nameof(id));
        }

        Id = id;
        CorrelationId = correlationId;
    }

    protected Entity() { }

    public Guid Id { get; private set; }
    public Guid? CorrelationId { get; private set; }
}
