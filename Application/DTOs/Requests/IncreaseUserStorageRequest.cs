namespace Application.DTOs.Requests
{
    public record IncreaseUserStorageRequest(Guid UserId, long Space);
}
