using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities
{
    public abstract class StorageMetadata : IHasDomainEvents
    {
        public Guid Id { get; protected set; }
        public string Name { get; set; }
        public Guid OwnerId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public string Path { get; set; }
        public DirectoryMetadata? Parent { get; set; } = null;

        protected readonly List<IDomainEvent> _domainEvents = new();
        public List<IDomainEvent> DomainEvents => _domainEvents;
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();

        public StorageMetadata()
        {
        }

        protected StorageMetadata(string name, Guid ownerId, string path, DirectoryMetadata? parent)
        {
            Id = Guid.NewGuid();
            Name = name;
            OwnerId = ownerId;
            CreatedAt = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(path))
            {
                Path = Id.ToString();
            }
            else
            {
                Path = System.IO.Path.Combine(path, Id.ToString());
            }
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
            Path = System.IO.Path.Combine(parent.Path, Id.ToString());
        }

        protected StorageMetadata(Guid id, string name, Guid ownerId, DirectoryMetadata? parent)
        {
            if (parent == null)
            {
                throw new DomainException("");
            }
            Id = id;
            Name = name;
            OwnerId = ownerId;
            CreatedAt = DateTime.UtcNow;
            Parent = parent;
            Path = System.IO.Path.Combine(parent.Path, Id.ToString());
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
