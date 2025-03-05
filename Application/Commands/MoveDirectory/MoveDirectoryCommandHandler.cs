using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.MoveDirectory
{
    public class MoveDirectoryCommandHandler : IRequestHandler<MoveDirectoryCommand>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IDirectoryService _directoryService;
        private readonly IUnitOfWork _unitOfWork;

        public MoveDirectoryCommandHandler(IDirectoryRepository directoryRepository, IDirectoryService directoryService, IUnitOfWork unitOfWork)
        {
            _directoryRepository = directoryRepository;
            _directoryService = directoryService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(MoveDirectoryCommand request, CancellationToken cancellationToken)
        {
            var from = await _directoryRepository.GetDirectoryAsync(request.FromId);
            var to = await _directoryRepository.GetDirectoryAsync(request.ToId);

            if (from is null || to is null)
            {
                throw new ApplicationException();
            }

            if (from.OwnerId != request.UserId || to.OwnerId != request.UserId)
            {
                throw new ApplicationException();
            }

            await _directoryService.MoveAsync(from.Path, to.Path, from.Id.ToString());

            await _directoryRepository.MoveDirectoryAsync(from, to);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
