using Domain.Events;
using MediatR;

namespace Application.Events.EventHandlers
{


    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            var user = notification.User;
            Console.WriteLine($"User created: {user.Email}");
            await Task.CompletedTask;
        }
    }
}
