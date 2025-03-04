using MediatR;

namespace Application.Commands.ChangeUserPassword
{
    public record ChangeUserPasswordCommand(string OldPassword, string NewPassword, string UserId) : IRequest;
}
