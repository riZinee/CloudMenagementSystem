using Application.Exceptions;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using System.Text.RegularExpressions;

namespace Application.Commands.ChangeUserPassword
{
    class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeUserPasswordCommandHandler(IUserRepository userRepository, IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(request.userId));

            if (user is null)
            {
                throw new ApplicationAuthorizationException(Messages.UserIsNotLogedIn);
            }

            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!]).{8,}$";
            Regex regex = new Regex(pattern);

            if (!regex.IsMatch(request.newPassword))
            {
                throw new ValidationDomainException(Messages.InvalidPasswordFormat);
            }

            var oldPasswordHash = _identityService.HashPassword(request.oldPassword, user.Salt);
            var newPasswordHash = _identityService.HashPassword(request.newPassword, user.Salt);

            user.ChangePassword(oldPasswordHash, newPasswordHash);

            _userRepository.Update(user);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
