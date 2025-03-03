using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public record UserLoginInEvent : IDomainEvent
    {
        public User User { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserLoginInEvent(User user)
        {
            User = user;
        }
    }
}
