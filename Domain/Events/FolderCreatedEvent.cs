using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public record FolderCreatedEvent : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
        public FolderMetadata Folder { get; set; }
        public User User { get; set; }

        public FolderCreatedEvent(FolderMetadata folder, User user)
        {
            Folder = folder;
            User = user;
        }
    }
}
