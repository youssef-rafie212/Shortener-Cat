using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.DTO.Analytics_DTOs;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AnalyticsRepo : IAnalyticsRepo
    {
        private readonly AppDbContext _db;
        private readonly IShortUrlsRepo _shortUrlsRepo;

        public AnalyticsRepo(AppDbContext db, IShortUrlsRepo shortUrlsRepo)
        {
            _db = db;
            _shortUrlsRepo = shortUrlsRepo;
        }

        public async Task<MaxVisitsByCountryDto?> GetMaxVisitsByCountry(int urlId)
        {
            MaxVisitsByCountryDto? res = await _db.UrlVisits.
               Where(v => v.ShortUrlId == urlId).
               GroupBy(v => v.Country).
               Select(g => new MaxVisitsByCountryDto()
               {
                   Country = g.Key ?? "Unknown",
                   Count = g.Count()
               }).
               OrderByDescending(m => m.Count).
               FirstOrDefaultAsync();

            return res;
        }

        public async Task<MaxVisitsByDeviceDto?> GetMaxVisitsByDevice(int urlId)
        {
            MaxVisitsByDeviceDto? res = await _db.UrlVisits.
               Where(v => v.ShortUrlId == urlId).
               GroupBy(v => v.DeviceType).
               Select(g => new MaxVisitsByDeviceDto()
               {
                   Device = g.Key,
                   Count = g.Count()
               }).
               OrderByDescending(m => m.Count).
               FirstOrDefaultAsync();

            return res;
        }

        // Get url with max visits (ik the name sucks)
        public async Task<MaxTotalVisitsForMultipleDto?> GetMaxVisitsForUserUrls(int userId)
        {
            List<ShortUrl> urlsOfUser = await _shortUrlsRepo.GetAllForUser(userId);
            List<int> urlsIdsOfUser = [];
            foreach (ShortUrl url in urlsOfUser)
            {
                urlsIdsOfUser.Add(url.Id);
            }

            var res = await _db.UrlVisits.
                Where(v => urlsIdsOfUser.Contains(v.ShortUrlId)).
                GroupBy(v => v.ShortUrlId).
                Select(g => new MaxTotalVisitsForMultipleDto()
                {
                    ShortUrl = g.Key.ToString(),
                    Count = g.Count()
                }).
                OrderByDescending(m => m.Count).
                FirstOrDefaultAsync();

            if (res == null) return null;

            ShortUrl maxShortUrl = (await _shortUrlsRepo.GetOneById(int.Parse(res.ShortUrl)))!;
            res.ShortUrl = maxShortUrl.Value;

            return res;
        }

        public async Task<List<VisitsByCountryDto>> GetVisitsByCountry(int urlId)
        {
            var res = await _db.UrlVisits.
                Where(v => v.ShortUrlId == urlId).
                GroupBy(v => v.Country).
                Select(g => new VisitsByCountryDto()
                {
                    Country = g.Key ?? "Uknown",
                    Count = g.Count(),
                    Visits = g.ToList()
                }).ToListAsync();

            return res;
        }

        public async Task<List<VisitsByDeviceDto>> GetVisitsByDevice(int urlId)
        {
            var res = await _db.UrlVisits.
                Where(v => v.ShortUrlId == urlId).
                GroupBy(v => v.DeviceType).
                Select(g => new VisitsByDeviceDto()
                {
                    Device = g.Key,
                    Count = g.Count(),
                    Visits = g.ToList()
                }).ToListAsync();

            return res;
        }

        public async Task<TotalVisitsDto> GetVisitsForUrl(int urlId)
        {
            var visits = await _db.UrlVisits.Where(v => v.ShortUrlId == urlId).ToListAsync();
            TotalVisitsDto totalVisits = new() { Count = visits.Count, Visits = visits };
            return totalVisits;
        }
    }
}
