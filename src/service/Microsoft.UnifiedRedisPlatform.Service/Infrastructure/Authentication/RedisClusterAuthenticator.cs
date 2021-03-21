using System;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;

namespace Microsoft.UnifiedPlatform.Service.Authentication
{
    public class RedisClusterAuthenticator: IAuthenticator
    {
        private readonly string _audience;
        private readonly string _issuer;
        private readonly string _secret;

        public RedisClusterAuthenticator(string audience, string issuer, string secret)
        {
            _audience = audience;
            _issuer = issuer;
            _secret = secret;
        }

        public Task<string> GenerateToken(string resourceId, Dictionary<string, string> additionalClaims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            if (additionalClaims != null && additionalClaims.Any())
            {
                foreach (var claim in additionalClaims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value));
                }
            }
            var jwtToken = new JwtSecurityToken(issuer: _issuer, audience: _audience, claims: claims, expires: DateTime.Now.AddMinutes(45), signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return Task.FromResult(token);
        }
    }
}
