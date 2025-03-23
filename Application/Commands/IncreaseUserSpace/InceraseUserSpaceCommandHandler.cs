using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.IncreaseUserSpace
{
    class InceraseUserSpaceCommandHandler : IRequestHandler<InceraseUserSpaceCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InceraseUserSpaceCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(InceraseUserSpaceCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
            {
                throw new ApplicationException();
            }

            user.IncreaseUserStorage(request.Space);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
