namespace Application.DTOs.Requests
{
    public record RemovePermissionRequest(Guid OwnerId, Guid StorageId, Guid UserId);
}
