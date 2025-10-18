using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pb403ApiDemo.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pb403ApiDemo.Services
{
    public class AuthService
    {
        private readonly JwtSettings _jwtSettings;

        public AuthService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        public async Task<JwtResponseModel> CreateToken(JwtRequestModel model)
        {
            var jwtSecurityToken = CreateJwtToken(model);

            return new JwtResponseModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
        }

        private JwtSecurityToken CreateJwtToken(JwtRequestModel model)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Username),
                    new Claim(JwtRegisteredClaimNames.Email, model.Email),
                    //new Claim("roles",string.Join(",",model.Roles))
                };

            claims.AddRange(model.Roles.Select(x => new Claim(ClaimTypes.Role, x)));

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
    }
}
