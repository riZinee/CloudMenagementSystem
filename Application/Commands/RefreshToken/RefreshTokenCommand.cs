using MediatR;

namespace Application.Commands.RefreshToken
{
    public record RefreshTokenCommand(string token, string refresh) : IRequest<string>;
}
