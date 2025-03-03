using Application.Interfaces;
using Domain.Events;
using MediatR;

namespace Application.Events.EventHandlers
{


    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly IEmailService _emailService;

        public UserCreatedEventHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            var user = notification.User;
            Console.WriteLine($"User created: {user.Email}");
            string activationLink = $"https://localhost:7132/api/auth/activate?token={user.ActivationToken}";

            string emailBody = $"<html><body>" +
                                  $"Click the link to activate your account: " +
                                  $"<a href='{activationLink}'>Activate</a>" +
                                  $"</body></html>";
            await _emailService.SendEmailAsync(user.Email.ToString(), "Confirm your registration", emailBody);

            await Task.CompletedTask;
        }
    }
}
