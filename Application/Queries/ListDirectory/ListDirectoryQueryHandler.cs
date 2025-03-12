using Application.DTOs.Responses;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Queries.ListDirectory
{
    public class ListDirectoryQueryHandler : IRequestHandler<ListDirectoryQuery, List<StorageResponse>>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IPermissionRepository _permissionRepository;

        public ListDirectoryQueryHandler(IDirectoryRepository directoryRepository, IPermissionRepository permissionRepository)
        {
            _directoryRepository = directoryRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<List<StorageResponse>> Handle(ListDirectoryQuery request, CancellationToken cancellationToken)
        {
            var directory = await _directoryRepository.GetDirectoryWithSubstorageAsync(request.DirectoryId);

            if (directory is null)
            {
                throw new ApplicationException();
            }

            var permission = await _permissionRepository.GetByUserAndStorageAsync(request.UserId, directory.Id);

            if (directory.OwnerId != request.UserId && !permission.Values.Contains(PermissionValue.Read))
            {
                throw new ApplicationException();
            }

            var storageList = new List<StorageResponse>();

            foreach (var storage in directory.SubStorage)
            {
                if (storage is FileMetadata file)
                {
                    storageList.Add(new StorageResponse(
                            file.Name,
                            "File",
                            file.ContentType,
                            file.Size
                        ));

                }
                else if (storage is DirectoryMetadata subDirectory)
                {
                    storageList.Add(new StorageResponse(
                            subDirectory.Name,
                            "Directory",
                            null,
                            null
                            ));
                }
            }

            return storageList;
        }
    }
}
