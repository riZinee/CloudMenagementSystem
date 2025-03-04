namespace Domain.Common
{
    public interface IHasDomainEvents
    {
        List<IDomainEvent> DomainEvents { get; }
        void ClearEvents();
    }
}
