using MediatR;

namespace Application.Commands.CreateUser
{
    public record CreateUserCommand(string Name, string Email, string Password) : IRequest<Guid>;
}

