using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ShortUrlsRepo : IShortUrlsRepo
    {
        private readonly AppDbContext _db;

        public ShortUrlsRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task Activate(ShortUrl shortUrl)
        {
            shortUrl.IsActive = true;
            await _db.SaveChangesAsync();
        }

        public async Task AddOne(ShortUrl shortUrl)
        {
            _db.ShortUrls.Add(shortUrl);
            await _db.SaveChangesAsync();
        }

        public async Task Deactivate(ShortUrl shortUrl)
        {
            shortUrl.IsActive = false;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteOne(ShortUrl shortUrl)
        {
            _db.ShortUrls.Remove(shortUrl);
            await _db.SaveChangesAsync();
        }

        public async Task<List<ShortUrl>> GetAllForUser(int userId)
        {
            return await _db.ShortUrls.Where(u => u.UserId == userId && u.ExpiresAt > DateTime.UtcNow).ToListAsync();
        }

        public async Task<ShortUrl?> GetOneById(int id)
        {
            ShortUrl? res = await _db.ShortUrls.Include("User").FirstOrDefaultAsync(u => u.Id == id && u.ExpiresAt > DateTime.UtcNow);
            return res;
        }

        public async Task<ShortUrl> UpdateOne(ShortUrl oldShortUrl, ShortUrl newShortUrl)
        {
            oldShortUrl.OriginalUrl = newShortUrl.OriginalUrl;
            oldShortUrl.Value = newShortUrl.Value;
            oldShortUrl.Code = newShortUrl.Code;
            oldShortUrl.VisitCount = newShortUrl.VisitCount;
            await _db.SaveChangesAsync();
            return oldShortUrl;
        }
    }
}
