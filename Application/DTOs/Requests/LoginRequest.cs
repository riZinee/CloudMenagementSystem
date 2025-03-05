namespace Application.DTOs.Requests
{
    public record LoginRequest(string NameOrEmail, string Password);
}
