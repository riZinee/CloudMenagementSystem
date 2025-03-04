using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.CreateHomeCatalog
{
    public class CreateHomeCatalogCommandHandler : IRequestHandler<CreateHomeCatalogCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateHomeCatalogCommandHandler(IUserRepository userRepository, IFolderRepository folderRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _folderRepository = folderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateHomeCatalogCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                throw new ApplicationNullException(Messages.FolderIsNull);
            }

            var folder = new FolderMetadata("home", request.UserId, request.Path);
            user.HomeCatalog = folder.Id;

            await _folderRepository.AddFolderAsync(folder);
            _userRepository.Update(user);

            await _unitOfWork.SaveChangesAsync();

            return folder.Id;
        }
    }
}
