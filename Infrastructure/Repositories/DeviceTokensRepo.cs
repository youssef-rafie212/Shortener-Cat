using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DeviceTokensRepo : IDeviceTokensRepo
    {
        private readonly AppDbContext _db;

        public DeviceTokensRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddOne(DeviceToken deviceToken)
        {
            _db.DeviceTokens.Add(deviceToken);
            await _db.SaveChangesAsync();
        }

        public async Task<List<DeviceToken>> GetAll()
        {
            return await _db.DeviceTokens.ToListAsync();
        }

        public async Task<DeviceToken?> GetOneById(int tokenId)
        {
            return await _db.DeviceTokens.FindAsync(tokenId);
        }

        public async Task<DeviceToken?> GetOneByToken(string deviceToken)
        {
            return await _db.DeviceTokens.FirstOrDefaultAsync(t => t.Value == deviceToken);
        }

        public async Task<DeviceToken?> GetOneByUserId(int userId)
        {
            return await _db.DeviceTokens.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task RemoveOne(DeviceToken deviceToken)
        {
            _db.DeviceTokens.Remove(deviceToken);
            await _db.SaveChangesAsync();
        }
    }
}
