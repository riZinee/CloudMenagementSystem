using Application.DTOs.Responses;
using MediatR;

namespace Application.Commands.LoginUser
{
    public record LoginCommand(string NameOrEmail, string Password) : IRequest<LoginResponse>;
}
