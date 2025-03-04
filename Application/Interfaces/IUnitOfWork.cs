namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        Task EnqueueDomainEventsAsync();
    }

}
