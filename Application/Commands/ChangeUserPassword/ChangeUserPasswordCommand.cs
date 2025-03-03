using MediatR;

namespace Application.Commands.ChangeUserPassword
{
    public record ChangeUserPasswordCommand(string oldPassword, string newPassword, string userId) : IRequest;
}
