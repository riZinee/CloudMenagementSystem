using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.ListStoragePermissions
{
    class ListPermisionsQueryHandler : IRequestHandler<ListPermisionsQuery, List<PermissionResponse>>
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ListPermisionsQueryHandler(IDirectoryRepository directoryRepository, IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
        {
            _directoryRepository = directoryRepository;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PermissionResponse>> Handle(ListPermisionsQuery request, CancellationToken cancellationToken)
        {
            var storage = await _directoryRepository.GetDirectoryAsync(request.StorageId);

            if (storage is null)
            {
                throw new ApplicationException();
            }

            if (storage.OwnerId != request.OwnerId)
            {
                throw new ApplicationException();
            }

            var permissionList = await _permissionRepository.GetByStorageAsync(request.StorageId);

            List<PermissionResponse> responseList = new();

            permissionList.ForEach(p => responseList.Add(new PermissionResponse(p.Id, p.UserId, p.Values)));

            return responseList;
        }
    }
}
