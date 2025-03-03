using MediatR;

namespace Application.Commands.ChangeUserName
{
    public record ChangeUserNameCommand(string name, string userId) : IRequest;
}
