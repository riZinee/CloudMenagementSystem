using Application.Entities;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, string>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IIdentityService _identityService;

        public RefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, IIdentityService identityService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _identityService = identityService;

        }

        public async Task<string> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var userId = _identityService.GetIdNameFromJwt(request.Token);

            var refreshToken = await _refreshTokenRepository.GetByUserIdAsync(userId);

            if (!_identityService.IsJwtGeneratedByThisServer(request.Token) || request.Token == null)
            {
                throw new ApplicationAuthorizationException(Messages.BadToken);
            }
            if (refreshToken.ExpiresAt < DateTime.Now || refreshToken.IsRevoked || refreshToken.Token != request.Refresh)
            {
                throw new ApplicationAuthorizationException(Messages.ExpiredToken);
            }

            return _identityService.GenerateJwtString(userId.ToString(), []);
        }
    }
}
