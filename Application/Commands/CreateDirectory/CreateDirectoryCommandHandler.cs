using Application.Exceptions;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.CreateDirectory
{
    public class CreateDirectoryCommandHandler : IRequestHandler<CreateDirectoryCommand, Guid>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDirectoryCommandHandler(IDirectoryRepository directoryRepository, IUnitOfWork unitOfWork)
        {
            _directoryRepository = directoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateDirectoryCommand request, CancellationToken cancellationToken)
        {
            var directory = await _directoryRepository.GetDirectoryAsync(request.ParentId);

            if (directory == null)
            {
                throw new ApplicationNullException(Messages.DirectoryIsNull);
            }

            var subDirectory = directory.CreateSubDirectory(request.Name, request.UserId);

            await _directoryRepository.AddDirectoryAsync(subDirectory);

            await _unitOfWork.SaveChangesAsync();

            return subDirectory.Id;
        }
    }
}
