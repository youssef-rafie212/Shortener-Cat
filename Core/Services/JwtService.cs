using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration config, UserManager<ApplicationUser> userManager, IExpiredTokensRepo expiredTokensRepo, ILogger<JwtService> logger)
        {
            _config = config;
            _userManager = userManager;
            _expiredRepo = expiredTokensRepo;
            _logger = logger;
        }

        public async Task ExpireToken(string token)
        {
            await _expiredRepo.AddOne(new() { Value = token });
            _logger.LogInformation($"New token expired with the value: {token}");
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

            string tokenString = handler.WriteToken(token);

            _logger.LogInformation($"New jwt token created with the value: {tokenString}");

            return tokenString;
        }

        public bool IsExpired(string token)
        {
            return _expiredRepo.HasOne(token);
        }
    }
}
