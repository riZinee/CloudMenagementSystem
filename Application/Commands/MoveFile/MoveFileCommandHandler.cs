using Application.Interfaces;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.MoveFile
{
    public class MoveFileHandler : IRequestHandler<MoveFileCommand>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileService _fileService;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MoveFileHandler(IDirectoryRepository directoryRepository, IFileRepository fileRepository, IFileService fileService, IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
        {
            _directoryRepository = directoryRepository;
            _fileRepository = fileRepository;
            _fileService = fileService;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(MoveFileCommand request, CancellationToken cancellationToken)
        {
            var from = await _fileRepository.GetAsync(request.FromId);
            var to = await _directoryRepository.GetDirectoryAsync(request.ToId);

            if (from is null || to is null)
            {
                throw new ApplicationException();
            }

            var permissionFrom = await _permissionRepository.GetByUserAndStorageAsync(request.UserId, from.Id);
            var permissionTo = await _permissionRepository.GetByUserAndStorageAsync(request.UserId, to.Id);

            if ((from.OwnerId != request.UserId && !permissionFrom.Values.Contains(PermissionValue.Modify)) ||
                (to.OwnerId != request.UserId && (!permissionTo.Values.Contains(PermissionValue.Write) && from.OwnerId != to.OwnerId)))
            {
                throw new ApplicationException();
            }

            await _fileService.MoveFileAsync($"{from.Path}.{from.ContentType}", to.Path, from.Id.ToString());

            _fileRepository.Move(from, to);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
