using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public record UserCreatedEvent : IDomainEvent
    {
        public User User { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserCreatedEvent(User user)
        {
            User = user;
        }
    }
}
