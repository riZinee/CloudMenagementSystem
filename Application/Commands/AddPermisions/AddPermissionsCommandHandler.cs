using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.AddPermisions
{
    public class AddPermissionsCommandHandler : IRequestHandler<AddPermissionsCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddPermissionsCommandHandler(IUserRepository userRepository, IDirectoryRepository directoryRepository, IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _directoryRepository = directoryRepository;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AddPermissionsCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            var storage = await _directoryRepository.GetDirectoryAsync(request.StorageId);

            //dodaj sprawdzanie dla pliku

            if (storage is null || user is null)
            {
                throw new ApplicationException();
            }

            if (request.OwnerId != storage.OwnerId)
            {
                throw new ApplicationException();
            }

            var permissions = new Permission(request.UserId, request.StorageId, request.Values);

            if (await _permissionRepository.GetByUserAndStorageAsync(request.UserId, request.StorageId) is not null)
            {
                throw new ApplicationException();
            }

            if (storage is DirectoryMetadata)
            {
                await _permissionRepository.AddPermissionsToSubStoragesAsync(permissions);
            }
            else
            {
                await _permissionRepository.AddAsync(permissions);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
