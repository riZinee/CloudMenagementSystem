using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.CreateHomeCatalog
{
    public class CreateHomeDirectoryCommandHandler : IRequestHandler<CreateHomeDirectoryCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IDirectoryService _directoryService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateHomeDirectoryCommandHandler(IUserRepository userRepository, IDirectoryRepository directoryRepository, IUnitOfWork unitOfWork, IDirectoryService directoryService)
        {
            _userRepository = userRepository;
            _directoryRepository = directoryRepository;
            _unitOfWork = unitOfWork;
            _directoryService = directoryService;
        }

        public async Task<Guid> Handle(CreateHomeDirectoryCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                throw new ApplicationNullException(Messages.DirectoryIsNull);
            }

            var directory = new DirectoryMetadata("home", request.UserId, request.Path);
            user.HomeCatalog = directory.Id;

            await _directoryRepository.AddDirectoryAsync(directory);
            _userRepository.Update(user);

            await _directoryService.CreateAsync(directory.Path);

            await _unitOfWork.SaveChangesAsync();

            return directory.Id;
        }
    }
}
