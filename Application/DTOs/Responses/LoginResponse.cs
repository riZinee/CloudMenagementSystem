namespace Application.DTOs.Responses
{
    public record LoginResponse(string Jwt, string RefreshToken);
}
