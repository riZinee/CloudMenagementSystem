using Application.Exceptions;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Queries.DownloadFile
{
    class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, Stream>
    {
        private readonly IFileService _fileService;
        private readonly IFileRepository _fileRepository;
        private readonly IPermissionRepository _permissionRepository;

        public DownloadFileQueryHandler(IFileService fileService, IFileRepository fileRepository, IPermissionRepository permissionRepository)
        {
            _fileService = fileService;
            _fileRepository = fileRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<Stream> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
        {
            var file = await _fileRepository.GetAsync(request.FileId);

            if (file == null)
            {
                throw new ApplicationNullException("");
            }

            var permission = await _permissionRepository.GetByUserAndStorageAsync(request.UserId, file.Id);

            if (file.OwnerId != request.UserId && !permission.Values.Contains(PermissionValue.Write))
            {
                throw new Exceptions.ApplicationException("");
            }

            return await _fileService.DownloadFileAsync(file.Path, file.ContentType);
        }
    }
}
