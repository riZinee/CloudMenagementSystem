using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.UpdatePermissions
{
    public class UpdatePermissionsCommandHandler : IRequestHandler<UpdatePermissionsCommand>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePermissionsCommandHandler(IPermissionRepository permissionRepository, IDirectoryRepository directoryRepository, IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _directoryRepository = directoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdatePermissionsCommand request, CancellationToken cancellationToken)
        {
            var storage = await _directoryRepository.GetDirectoryAsync(request.StorageId);

            //dodaj sprawdzanie dla pliku

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

            permissions.Values = request.Values;

            if (storage is DirectoryMetadata)
            {
                await _permissionRepository.UpdatePermissionsForSubStoragesAsync(permissions);
            }

            _permissionRepository.Update(permissions);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
