using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Poshta.Application.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Poshta.Infrastructure.Jwt
{
    public class JwtProvider(
        IOptions<JwtOptions> jwtOptions) : IJwtProvider
    {
        private readonly JwtOptions jwtOptions = jwtOptions.Value;

        public string Generate(Guid userId, IList<string> roles, IDictionary<string, string> additionalClaims)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach(var additionalClaim in additionalClaims)
            {
                claims.Add(new Claim(additionalClaim.Key, additionalClaim.Value));
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(jwtOptions.ExpiresHours),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
