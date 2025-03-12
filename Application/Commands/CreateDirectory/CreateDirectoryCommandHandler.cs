using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.CreateDirectory
{
    public class CreateDirectoryCommandHandler : IRequestHandler<CreateDirectoryCommand, Guid>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IDirectoryService _directoryService;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDirectoryCommandHandler(IDirectoryRepository directoryRepository, IUnitOfWork unitOfWork, IDirectoryService directoryService, IPermissionRepository permissionRepository)
        {
            _directoryRepository = directoryRepository;
            _unitOfWork = unitOfWork;
            _directoryService = directoryService;
            _permissionRepository = permissionRepository;
        }

        public async Task<Guid> Handle(CreateDirectoryCommand request, CancellationToken cancellationToken)
        {
            var directory = await _directoryRepository.GetDirectoryWithSubstorageAsync(request.ParentId);

            if (directory == null)
            {
                throw new ApplicationNullException(Messages.DirectoryIsNull);
            }

            var permission = await _permissionRepository.GetByUserAndStorageAsync(request.UserId, directory.Id);

            if (directory.OwnerId != request.UserId && !permission.Values.Contains(PermissionValue.Write))
            {
                throw new System.ApplicationException("");
            }

            var name = request.Name;
            int counter = 0;

            while (directory.SubStorage.Any(s => s.Name == name))
            {
                name += $"({++counter})";
            }

            var subDirectory = directory.CreateSubDirectory(name);

            await _directoryRepository.AddDirectoryAsync(subDirectory);

            await _directoryService.CreateAsync(subDirectory.Path);

            var permissionList = await _permissionRepository.GetByStorageAsync(request.ParentId);
            foreach (var p in permissionList)
            {
                await _permissionRepository.AddAsync(new Permission(
                    p.UserId,
                    subDirectory.Id,
                    p.Values));
            }

            await _unitOfWork.SaveChangesAsync();

            return subDirectory.Id;
        }
    }
}
