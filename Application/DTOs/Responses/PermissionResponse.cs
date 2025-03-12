using Domain.ValueObjects;

namespace Application.DTOs.Responses
{
    public record PermissionResponse(Guid Id, Guid UserId, List<PermissionValue> Values);
}
