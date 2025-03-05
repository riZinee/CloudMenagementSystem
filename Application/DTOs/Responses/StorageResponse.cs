namespace Application.DTOs.Responses
{
    public record StorageResponse(string Name, string Type, string? ContentType, long? Size);
}
