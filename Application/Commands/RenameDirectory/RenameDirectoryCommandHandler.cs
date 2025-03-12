using Application.Interfaces;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.RenameDirectory
{
    class RenameDirectoryCommandHandler : IRequestHandler<RenameDirectoryCommand>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RenameDirectoryCommandHandler(IDirectoryRepository directoryRepository, IUnitOfWork unitOfWork, IPermissionRepository permissionRepository)
        {
            _directoryRepository = directoryRepository;
            _unitOfWork = unitOfWork;
            _permissionRepository = permissionRepository;
        }

        public async Task Handle(RenameDirectoryCommand request, CancellationToken cancellationToken)
        {
            var directory = await _directoryRepository.GetDirectoryAsync(request.DirectoryId);

            if (directory is null)
            {
                throw new ApplicationException();
            }

            var permission = await _permissionRepository.GetByUserAndStorageAsync(request.UserId, directory.Id);

            if (directory.OwnerId != request.UserId && !permission.Values.Contains(PermissionValue.Modify))
            {
                throw new ApplicationException();
            }

            directory.Name = request.Name;

            _directoryRepository.Update(directory);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
