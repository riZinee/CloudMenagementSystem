namespace Application.DTOs.Requests
{
    public record CreateHomeDirectoryRequest(string Path, Guid UserId);
}
