using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IExpiredTokensRepo _expiredRepo;

        public JwtService(IConfiguration config, UserManager<ApplicationUser> userManager, IExpiredTokensRepo expiredTokensRepo)
        {
            _config = config;
            _userManager = userManager;
            _expiredRepo = expiredTokensRepo;
        }

        public async Task ExpireToken(string token)
        {
            await _expiredRepo.AddOne(new() { Value = token });
        }

        public string GenerateJwtToken(ApplicationUser user)
        {
            var jwtConfig = _config.GetSection("JWT");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));

            List<Claim> claims = [
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.UserName!),
            ];

            var roles = _userManager.GetRolesAsync(user).Result;

            foreach (var role in roles)
            {
                claims.Add(new(ClaimTypes.Role, role));
            }

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var descriptor = new SecurityTokenDescriptor()
            {
                Expires = DateTime.UtcNow.AddDays(7),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Audience = jwtConfig["Aud"],
                Issuer = jwtConfig["Issuer"]
            };

            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }
    }
}
