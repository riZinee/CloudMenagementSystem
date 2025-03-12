using MediatR;

namespace Application.Commands.RemovePermissions
{
    public record RemovePermissionsCommand(Guid OwnerId, Guid UserId, Guid StorageId) : IRequest;
}
