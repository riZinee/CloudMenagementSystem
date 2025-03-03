using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.LoginUser
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, LoginDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IIdentityService _identityService;

        public UserLoginCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _identityService = identityService;

        }
        public async Task<LoginDTO> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var name = request.Name;
            var email = request.Email;
            var password = request.Password;

            User user;

            if (string.IsNullOrWhiteSpace(email))
            {
                user = await _userRepository.GetByNameAsync(name);
            }
            else
            {
                user = await _userRepository.GetByEmailAsync(new Email(email));
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
            var token = _identityService.GenerateJwtString(user.Id.ToString(), []);

            _userRepository.Update(user);
            await _refreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new LoginDTO(token, refreshToken.Token);
        }
    }
}
