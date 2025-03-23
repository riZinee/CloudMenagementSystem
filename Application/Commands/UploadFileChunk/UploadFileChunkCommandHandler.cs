using Application;
using Application.Commands.UploadFileChunk;
using Application.Entities;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

public class UploadFileChunkCommandHandler : IRequestHandler<UploadFileChunkCommand, Guid>
{
    private readonly IFileService _storageService;
    private readonly IFileUploadRepository _fileUploadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileRepository _fileRepository;
    private readonly IDirectoryRepository _directoryRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRepository _userRepository;

    public UploadFileChunkCommandHandler
        (
        IFileService storageService,
        IFileUploadRepository fileUploadRepository,
        IUnitOfWork unitOfWork, IFileRepository fileRepository,
        IDirectoryRepository directoryRepository,
        IPermissionRepository permissionRepository,
        IUserRepository userRepository
        )
    {
        _storageService = storageService;
        _fileUploadRepository = fileUploadRepository;
        _unitOfWork = unitOfWork;
        _fileRepository = fileRepository;
        _directoryRepository = directoryRepository;
        _permissionRepository = permissionRepository;
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(UploadFileChunkCommand request, CancellationToken cancellationToken)
    {
        var directory = await _directoryRepository.GetDirectoryWithSubstorageAsync(request.DirectoryId);

        if (directory == null)
        {
            throw new Application.Exceptions.ApplicationException(Messages.DirectoryIsNull);
        }

        var permission = await _permissionRepository.GetByUserAndStorageAsync(request.UserId, directory.Id);

        if (directory.OwnerId != request.UserId && !permission.Values.Contains(PermissionValue.Write))
        {
            throw new Application.Exceptions.ApplicationException("");
        }

        if (directory.SubStorage.Any(s => s.Name == request.FileName))
        {
            throw new Application.Exceptions.ApplicationException("");
        }

        var upload = await _fileUploadRepository.GetByIdAsync(request.UploadId);

        if (upload == null)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (!user.UserStorage.CanStoreFile(request.FileSize))
            {
                throw new Application.Exceptions.ApplicationException("");
            }

            user.IncreaseUsedSpace(request.FileSize);

            upload = new FileUpload
            (
                request.UploadId,
                request.FileName,
                request.DirectoryId,
                request.TotalChunks,
                0,
                request.UserId,
                DateTime.UtcNow,
                null,
                false
            );

            _userRepository.Update(user);

            await _fileUploadRepository.AddAsync(upload);
        }

        var guid = await _storageService.UploadFileChunkAsync(
            request.UploadId,
            request.FileStream,
            Path.Combine(directory.Path, request.FileName),
            request.ChunkIndex, request.TotalChunks,
            request.UserId);

        upload.UploadedChunks++;

        if (upload.UploadedChunks == upload.TotalChunks)
        {
            upload.IsCompleted = true;
            upload.EndTime = DateTime.UtcNow;

            var file = new FileMetadata(
                guid,
                request.UserId,
                request.FileName,
                request.FileSize,
                request.FileName.Split(".").Last(),
                directory);

            await _fileRepository.AddAsync(file);

            var permissions = await _permissionRepository.GetByStorageAsync(request.DirectoryId);

            foreach (var p in permissions)
            {
                await _permissionRepository.AddAsync(new Permission(
                    p.UserId,
                    file.Id,
                    p.Values));
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return request.UploadId;
    }
}
