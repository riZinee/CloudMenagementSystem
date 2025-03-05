using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.DeleteDirectory
{
    public class DeleteDirectoryCommandHandler : IRequestHandler<DeleteDirectoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IDirectoryService _directoryService;

        public DeleteDirectoryCommandHandler(IUnitOfWork unitOfWork, IDirectoryRepository directoryRepository, IDirectoryService directoryService)
        {
            _unitOfWork = unitOfWork;
            _directoryRepository = directoryRepository;
            _directoryService = directoryService;
        }

        public async Task Handle(DeleteDirectoryCommand request, CancellationToken cancellationToken)
        {
            var directory = await _directoryRepository.GetDirectoryAsync(request.DirectoryId);

            if (directory is null)
            {
                throw new ApplicationException();
            }

            if (request.UserId != directory.OwnerId)
            {
                throw new DomainException();
            }

            await _directoryService.DeleteAsync(directory.Path);

            await _directoryRepository.DeleteDirectoryAsync(directory.Id);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
