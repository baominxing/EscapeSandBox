using EscapeSandBox.Api.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EscapeSandBox.Api.Core
{
    public class TokenHelper : ITokenHelper
    {
        public TokenHelper()
        {
        }

        public Token CreateToken(Agent agent)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.DateOfBirth ,"20")
            };

            return CreateToken(claims);
        }
        private Token CreateToken(Claim[] claims)
        {
            var now = DateTime.Now;
            var expires = now.Add(TimeSpan.FromMinutes(ApiConfig.AccessTokenExpiresMinutes));
            var token = new JwtSecurityToken(
                issuer: ApiConfig.Issuer,
                audience: ApiConfig.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiConfig.IssuerSigningKey)), SecurityAlgorithms.HmacSha256));
            return new Token { TokenContent = new JwtSecurityTokenHandler().WriteToken(token), Expires = expires };
        }
    }

    public interface ITokenHelper
    {
        Token CreateToken(Agent agent);
    }

    public class Token
    {
        public string TokenContent { get; set; }

        public DateTime Expires { get; set; }
    }
}
