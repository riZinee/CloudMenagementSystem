using Application.Entities;
using Application.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration _configuration;

        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secret;

        private readonly DateTime _now;

        private readonly int _iterationCountOfHashPassword = 10000;
        private readonly int _timeInMinutesValidJWT = 120;
        private readonly int _timeInHourValidRefreshToken = 48;

        private readonly string _personRole = "person";
        private readonly string _companyRole = "company";


        //Constructor
        public IdentityService
            (

            IConfiguration configuration
            )
        {
            _configuration = configuration;

            var jwtSection = _configuration.GetSection("JwtData");

            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var secret = jwtSection["Secret"];

            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ApplicationException(Messages.UserSecrets_Issuer_NotConfigured);
            }
            if (string.IsNullOrWhiteSpace(audience))
            {
                throw new ApplicationException(Messages.UserSecrets_Audience_NotConfigured);
            }
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new ApplicationException(Messages.UserSecrets_Secret_NotConfigured);
            }

            _issuer = issuer;
            _audience = audience;
            _secret = secret;
        }

        public string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public string HashPassword(string password, string salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: password,
               salt: Convert.FromBase64String(salt),
               prf: KeyDerivationPrf.HMACSHA1,
               iterationCount: _iterationCountOfHashPassword,
               numBytesRequested: 256 / 8
               ));
        }

        public RefreshToken GenerateRefreshToken(Guid id)
        {
            byte[] salt = new byte[1024];
            using (var genNum = RandomNumberGenerator.Create())
            {
                genNum.GetBytes(salt);
            }
            var token = Convert.ToBase64String(salt);
            var valid = DateTime.Now.AddHours(_timeInHourValidRefreshToken);
            return new RefreshToken(id, token, valid);
        }

        public string GenerateJwtString
            (
            string name,
            IEnumerable<string> roles
            )
        {
            var claims = GenerateClaims(name, roles);
            var token = GenerateJWT(claims);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        public bool IsJwtGeneratedByThisServer(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            try
            {
                tokenHandler.ValidateToken(jwt, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = secretKey,
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsJwtGeneratedByThisServerAndNotExpired(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            try
            {
                tokenHandler.ValidateToken(jwt, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = secretKey,
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetPersonRole() => _personRole;
        public string GetCompanyRole() => _companyRole;

        public Guid GetIdNameFromJwt(string jwt)
        {
            if (!IsJwtGeneratedByThisServer(jwt))
            {
                throw new ApplicationException();
                //UserException
                //(
                //Messages.User_Jwt_IsNotGeneratedByThisServer,
                //DomainExceptionTypeEnum.Unauthorized
                //);
            }
            var claims = GetClaimsFromJwt(jwt);
            var id = GetIdNameFromClaims(claims);
            return id;
        }

        public Guid GetIdNameFromClaims(IEnumerable<Claim> claims)
        {
            var name = GetNameFromClaims(claims);
            if (!Guid.TryParse(name, out var id))
            {
                throw new ApplicationException();
                //UserException
                //(
                //$"{Messages.User_Jwt_NameIsNotGuid}: {name}",
                //DomainExceptionTypeEnum.AppProblem
                //);
            }
            return id;
        }

        private JwtSecurityToken GenerateJWT(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            var signing = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims.ToArray(),
                expires: DateTime.Now.ToLocalTime().AddMinutes(_timeInMinutesValidJWT),
                signingCredentials: signing
             );
        }

        private IEnumerable<Claim> GenerateClaims(string userId, IEnumerable<string> roles)
        {
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,userId),
            };

            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            return claims;
        }

        private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(jwt);
            var claims = jwtToken.Claims.ToList();
            return claims;
        }

        private string GetNameFromClaims(IEnumerable<Claim> claims)
        {
            var name = "";
            foreach (var claim in claims)
            {
                if (claim.Type == ClaimTypes.NameIdentifier)
                {
                    name = claim.Value;
                    break;
                }
            }
            return name;
        }
    }
}
