using Application.DTOs;
using MediatR;

namespace Application.Commands.LoginUser
{
    public record UserLoginCommand(string NameOrEmail, string Password) : IRequest<LoginDTO>;
}
