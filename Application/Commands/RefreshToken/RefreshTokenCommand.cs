using MediatR;

namespace Application.Commands.RefreshToken
{
    public record RefreshTokenCommand(string Token, string Refresh) : IRequest<string>;
}
