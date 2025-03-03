using Application.Entities;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IIdentityService
    {
        string GenerateSalt();
        string HashPassword(string password, string salt);
        RefreshToken GenerateRefreshToken(Guid id);
        string GenerateJwtString(string name, IEnumerable<string> roles);
        bool IsJwtGeneratedByThisServer(string jwt);
        bool IsJwtGeneratedByThisServerAndNotExpired(string jwt);
        string GetPersonRole();
        string GetCompanyRole();
        Guid GetIdNameFromJwt(string jwt);
        Guid GetIdNameFromClaims(IEnumerable<Claim> claims);
    }
}
