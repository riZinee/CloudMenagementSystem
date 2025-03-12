using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.RemovePermissions
{
    public class RemovePermissionsCommandHandler : IRequestHandler<RemovePermissionsCommand>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemovePermissionsCommandHandler(IPermissionRepository permissionRepository, IDirectoryRepository directoryRepository, IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _directoryRepository = directoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RemovePermissionsCommand request, CancellationToken cancellationToken)
        {
            var storage = await _directoryRepository.GetDirectoryAsync(request.StorageId);

            if (storage is null)
            {
                throw new ApplicationException();
            }

            if (storage.OwnerId != request.OwnerId)
            {
                throw new ApplicationException();
            }

            var permissions = await _permissionRepository.GetByUserAndStorageAsync(request.UserId, request.StorageId);

            if (permissions is null)
            {
                throw new ApplicationException();
            }

            await _permissionRepository.RemovePermissionsAsync(permissions.Id);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
