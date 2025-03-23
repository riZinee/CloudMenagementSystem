using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Queries.GetFile
{
    class GetFileQueryHandler : IRequestHandler<GetFileQuery, FileMetadata>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IPermissionRepository _permissionRepository;

        public GetFileQueryHandler(IFileRepository fileRepository, IPermissionRepository permissionRepository)
        {
            _fileRepository = fileRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<FileMetadata> Handle(GetFileQuery request, CancellationToken cancellationToken)
        {
            var file = await _fileRepository.GetAsync(request.FileId);
            var permission = await _permissionRepository.GetByUserAndStorageAsync(request.UserId, request.FileId);

            if (file.OwnerId != request.UserId && !permission.Values.Contains(PermissionValue.Read))
            {
                throw new Exceptions.ApplicationException("");
            }

            return file;
        }
    }
}
