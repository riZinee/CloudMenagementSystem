namespace Application.DTOs.Requests
{
    public record RefreshTokenRequest(string Jwt, string RefreshToken);
}
