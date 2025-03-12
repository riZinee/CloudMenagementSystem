using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.AddPermisions
{
    public record AddPermissionsCommand(Guid OwnerId, Guid UserId, Guid StorageId, List<PermissionValue> Values) : IRequest;
}
