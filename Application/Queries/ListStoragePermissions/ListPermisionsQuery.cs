using Application.DTOs.Responses;
using MediatR;

namespace Application.Queries.ListStoragePermissions
{
    public record ListPermisionsQuery(Guid OwnerId, Guid StorageId) : IRequest<List<PermissionResponse>>;
}
