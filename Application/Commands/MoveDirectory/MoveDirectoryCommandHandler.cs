using Application.Interfaces;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.MoveDirectory
{
    public class MoveDirectoryCommandHandler : IRequestHandler<MoveDirectoryCommand>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IDirectoryService _directoryService;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MoveDirectoryCommandHandler(IDirectoryRepository directoryRepository, IDirectoryService directoryService, IUnitOfWork unitOfWork, IPermissionRepository permissionRepository)
        {
            _directoryRepository = directoryRepository;
            _directoryService = directoryService;
            _unitOfWork = unitOfWork;
            _permissionRepository = permissionRepository;
        }

        public async Task Handle(MoveDirectoryCommand request, CancellationToken cancellationToken)
        {
            var from = await _directoryRepository.GetDirectoryAsync(request.FromId);
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

            await _directoryService.MoveAsync(from.Path, to.Path, from.Id.ToString());

            await _directoryRepository.MoveDirectoryAsync(from, to);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
