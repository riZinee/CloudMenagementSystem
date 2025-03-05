using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.RenameDirectory
{
    class RenameDirectoryCommandHandler : IRequestHandler<RenameDirectoryCommand>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RenameDirectoryCommandHandler(IDirectoryRepository directoryRepository, IUnitOfWork unitOfWork)
        {
            _directoryRepository = directoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RenameDirectoryCommand request, CancellationToken cancellationToken)
        {
            var directory = await _directoryRepository.GetDirectoryAsync(request.DirectoryId);

            if (directory is null)
            {
                throw new ApplicationException();
            }

            if (directory.OwnerId != request.UserId)
            {
                throw new ApplicationException();
            }

            directory.Name = request.Name;

            _directoryRepository.Update(directory);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
