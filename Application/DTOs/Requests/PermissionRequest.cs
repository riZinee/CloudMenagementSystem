using Domain.ValueObjects;

namespace Application.DTOs.Requests
{
    public record PermissionRequest(Guid StorageId, Guid UserId, List<PermissionValue> Values);
}
