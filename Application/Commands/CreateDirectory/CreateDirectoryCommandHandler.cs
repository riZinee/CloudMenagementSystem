using Application.Exceptions;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.CreateDirectory
{
    public class CreateDirectoryCommandHandler : IRequestHandler<CreateDirectoryCommand, Guid>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IDirectoryService _directoryService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDirectoryCommandHandler(IDirectoryRepository directoryRepository, IUnitOfWork unitOfWork, IDirectoryService directoryService)
        {
            _directoryRepository = directoryRepository;
            _unitOfWork = unitOfWork;
            _directoryService = directoryService;
        }

        public async Task<Guid> Handle(CreateDirectoryCommand request, CancellationToken cancellationToken)
        {
            var directory = await _directoryRepository.GetDirectoryAsync(request.ParentId);

            if (directory == null)
            {
                throw new ApplicationNullException(Messages.DirectoryIsNull);
            }

            if (directory.OwnerId != request.UserId)
            {
                throw new System.ApplicationException("");
            }

            var subDirectory = directory.CreateSubDirectory(request.Name);

            await _directoryRepository.AddDirectoryAsync(subDirectory);

            await _directoryService.CreateAsync(subDirectory.Path);

            await _unitOfWork.SaveChangesAsync();

            return subDirectory.Id;
        }
    }
}
