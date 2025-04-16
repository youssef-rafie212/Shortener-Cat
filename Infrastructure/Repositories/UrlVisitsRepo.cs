using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UrlVisitsRepo : IUrlVisitsRepo
    {
        private readonly AppDbContext _db;

        public UrlVisitsRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddOne(UrlVisit visit)
        {
            _db.UrlVisits.Add(visit);
            await _db.SaveChangesAsync();
        }

        public async Task<List<UrlVisit>> GetAllVisitsForUrl(int shortUrlId)
        {
            return await _db.UrlVisits.Where(v => v.ShortUrlId == shortUrlId).ToListAsync();
        }

        public async Task<UrlVisit?> GetOneById(int id)
        {
            return await _db.UrlVisits.FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}
