using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Infrastructure.DB;

namespace Infrastructure.Repositories
{
    public class ExpiredTokensRepo : IExpiredTokensRepo
    {
        private readonly AppDbContext _db;

        public ExpiredTokensRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddOne(ExpiredToken token)
        {
            _db.ExpiredTokens.Add(token);
            await _db.SaveChangesAsync();
        }
    }
}
