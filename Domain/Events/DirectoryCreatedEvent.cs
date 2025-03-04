using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public record DirectoryCreatedEvent : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
        public DirectoryMetadata Directory { get; set; }
        public User User { get; set; }

        public DirectoryCreatedEvent(DirectoryMetadata directory, User user)
        {
            Directory = directory;
            User = user;
        }
    }
}
