using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public record UserAccountActivatedEvent(Guid UserId, User User) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserAccountActivatedEvent(Guid userId, User user, DateTime occurredOn) : this(userId, user)
        {
        }
    }
}
