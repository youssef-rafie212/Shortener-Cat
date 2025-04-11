using Core.Domain.Entities;

namespace Core.ServicesContracts
{
    public interface IJwtService
    {
        string GenerateJwtToken(ApplicationUser user);
    }
}
