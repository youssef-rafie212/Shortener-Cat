using Core.Domain.Entities;

namespace Core.ServicesContracts
{
    public interface IJwtService
    {
        string GenerateJwtToken(ApplicationUser user);
        Task ExpireToken(string token);
        bool IsExpired(string token);
    }
}
