using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.UpdatePermissions
{
    public record UpdatePermissionsCommand(Guid OwnerId, Guid UserId, Guid StorageId, List<PermissionValue> Values) : IRequest;
}
