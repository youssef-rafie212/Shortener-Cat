using Core.Domain.Entities;

namespace Core.Domain.RepositoryContracts
{
    public interface IExpiredTokensRepo
    {
        Task AddOne(ExpiredToken token);
        bool HasOne(string token);
    }
}
