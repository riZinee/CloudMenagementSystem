using Application.Exceptions;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.CreateFolder
{
    public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, Guid>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateFolderCommandHandler(IFolderRepository folderRepository, IUnitOfWork unitOfWork)
        {
            _folderRepository = folderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.GetFolderAsync(request.ParentId);

            if (folder == null)
            {
                throw new ApplicationNullException(Messages.FolderIsNull);
            }

            var subFolder = folder.CreateSubFolder(request.Name, request.UserId);

            await _folderRepository.AddFolderAsync(subFolder);

            await _unitOfWork.SaveChangesAsync();

            return subFolder.Id;
        }
    }
}
