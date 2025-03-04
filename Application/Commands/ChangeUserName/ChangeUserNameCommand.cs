using MediatR;

namespace Application.Commands.ChangeUserName
{
    public record ChangeUserNameCommand(string Name, string UserId) : IRequest;
}
