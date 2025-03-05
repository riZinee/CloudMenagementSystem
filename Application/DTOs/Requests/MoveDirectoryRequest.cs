namespace Application.DTOs.Requests
{
    public record MoveDirectoryRequest(Guid FromId, Guid ToId);
}
