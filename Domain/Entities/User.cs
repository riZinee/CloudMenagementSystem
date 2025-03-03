using Domain.Common;
using Domain.Events;
using Domain.ValueObjects;
namespace Domain.Entities
{
    public class User : IHasDomainEvents
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Email Email { get; private set; }
        public UserStorage UserStorage { get; private set; }
        public Guid HomeCatalog { get; set; }
        public Role Role { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private User() { }

        public User(string name, Email email, string password)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PasswordHash = password;
            UserStorage = new UserStorage(0);
            Role = Role.User;

            _domainEvents.Add(new UserCreatedEvent(this));
        }

        public void AddHomeCatalog(Guid folderId)
        {
            HomeCatalog = folderId;
        }

        public void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}
