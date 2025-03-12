using Domain.Common;
using Domain.Events;
using Domain.Exceptions;
using Domain.ValueObjects;
namespace Domain.Entities
{
    public class User : IHasDomainEvents
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public Guid Id { get; private set; }
        public string Name { get; set; }
        public Email Email { get; private set; }
        public UserStorage UserStorage { get; private set; }
        public Guid HomeCatalog { get; set; }
        public Role Role { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public bool IsActive { get; private set; }
        public string? ActivationToken { get; private set; } = null;
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();


        public List<IDomainEvent> DomainEvents => _domainEvents;

        private User() { }

        public User(string name, Email email, string password, string salt)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PasswordHash = password;
            UserStorage = new UserStorage(0);
            Role = Role.User;
            Salt = salt;
            IsActive = false;
            ActivationToken = Guid.NewGuid().ToString();

            _domainEvents.Add(new UserCreatedEvent(this));
        }

        public void AddHomeCatalog(Guid directoryId)
        {
            HomeCatalog = directoryId;
        }

        public void ChangeEmail(string email)
        {
            Email = new Email(email);
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (oldPassword != PasswordHash)
            {
                throw new DomainException(Messages.InvalidPassword);
            }

            PasswordHash = newPassword;
        }

        public void IncreaseUserStorage(long space)
        {
            if (space < 0)
            {
                throw new DomainException(Messages.SpaceIsNegative);
            }

            UserStorage = new UserStorage(UserStorage.UsedSpace, UserStorage.TotalSpace + space);
        }
        public void DecreaseUsedSpace(long fileSize)
        {
            UserStorage = new UserStorage(Math.Max(0, UserStorage.UsedSpace - fileSize), UserStorage.TotalSpace);
        }

        public void Activate()
        {
            IsActive = true;
            ActivationToken = null;
        }

        public void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}
