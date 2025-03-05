using Application.DTOs.Responses;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.LoginUser
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IIdentityService _identityService;

        public LoginCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _identityService = identityService;

        }
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var password = request.Password;

            User user;

            user = await _userRepository.GetByNameAsync(request.NameOrEmail);

            if (user is null)
            {
                user = await _userRepository.GetByEmailAsync(new Email(request.NameOrEmail));
            }

            if (user is null)
            {
                throw new ApplicationAuthorizationException(Messages.UserIsNotLogedIn);
            }

            if (_identityService.HashPassword(password, user.Salt) != user.PasswordHash)
            {
                throw new ApplicationAuthorizationException(Messages.UserIsNotLogedIn);
            }

            if (!user.IsActive)
            {
                throw new ApplicationAuthorizationException(Messages.UserIsNotActivated);
            }

            var refreshToken = _identityService.GenerateRefreshToken(user.Id);
            var token = _identityService.GenerateJwtString(user.Id.ToString(), [user.Role.ToString()]);

            _userRepository.Update(user);
            await _refreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponse(token, refreshToken.Token);
        }
    }
}
