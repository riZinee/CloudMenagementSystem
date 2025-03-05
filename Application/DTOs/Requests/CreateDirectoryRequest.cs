namespace Application.DTOs.Requests
{
    public record CreateDirectoryRequest(string Name, Guid ParentId);
}
