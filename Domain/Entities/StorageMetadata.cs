using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities
{
    public abstract class StorageMetadata : IHasDomainEvents
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public Guid OwnerId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public string Path { get; set; }
        public DirectoryMetadata? Parent { get; set; }

        protected readonly List<IDomainEvent> _domainEvents = new();
        public List<IDomainEvent> DomainEvents => _domainEvents;

        public StorageMetadata()
        {
        }

        protected StorageMetadata(string name, Guid ownerId, string path, DirectoryMetadata? parent)
        {
            Id = Guid.NewGuid();
            Name = name;
            OwnerId = ownerId;
            CreatedAt = DateTime.UtcNow;
            Path = path;
            Parent = parent;
        }

        protected StorageMetadata(string name, Guid ownerId, DirectoryMetadata? parent)
        {
            if (parent == null)
            {
                throw new DomainException("");
            }
            Id = Guid.NewGuid();
            Name = name;
            OwnerId = ownerId;
            CreatedAt = DateTime.UtcNow;
            Parent = parent;
            Path = $"{parent.Path}/{Name}";
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}
