namespace Application.DTOs.Response
{
    public record UserResponse(string Name, string Email, long UsedSpace, long TotalSpace);
}
