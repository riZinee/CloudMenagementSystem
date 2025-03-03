using Application.Exceptions;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.ChangeUserName
{
    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserNameCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeUserPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ChangeUserNameCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(request.userId));

            if (user is null)
            {
                throw new ApplicationAuthorizationException(Messages.UserIsNotLogedIn);
            }

            var existingUser = await _userRepository.GetByNameAsync(request.name);

            if (existingUser is not null)
            {
                throw new DomainEntityAlreadyExistsException(Messages.NameIsAlreadyTaken);
            }

            user.Name = request.name;

            _userRepository.Update(user);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
