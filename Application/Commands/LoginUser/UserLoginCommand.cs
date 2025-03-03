using Application.DTOs;
using MediatR;

namespace Application.Commands.LoginUser
{
    public record UserLoginCommand(string Name, string Email, string Password) : IRequest<LoginDTO>;
}
