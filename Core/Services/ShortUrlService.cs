using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Core.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IShortUrlsRepo _repo;
        private readonly IShortenerService _shortenerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDistributedCache _cache;

        public ShortUrlService(
            IShortUrlsRepo repo,
            IShortenerService shortenerService,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IDistributedCache cache
            )
        {
            _repo = repo;
            _shortenerService = shortenerService;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _cache = cache;
        }

        public async Task Activate(int id)
        {
            ShortUrl? target = await _repo.GetOneById(id);
            if (target == null) throw new Exception("URL does not exist");
            await _repo.Activate(target);
            await _cache.SetStringAsync(id.ToString(), JsonSerializer.Serialize(target));
        }

        public async Task<ShortUrl> CreateShortUrl(int userId, string originalUrl)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new Exception("User does not exist");

            ShortUrl shortUrl = new()
            {
                OriginalUrl = originalUrl,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                User = user,
                UserId = userId
            };

            await _repo.AddOne(shortUrl);

            string code = _shortenerService.Encode(shortUrl.Id);
            string protocol = _contextAccessor.HttpContext!.Request.Scheme;
            string host = _contextAccessor.HttpContext!.Request.Host.ToString();
            string value = $"{protocol}://{host}/{code}";

            shortUrl.Code = code;
            shortUrl.Value = value;
            await _repo.UpdateOne(shortUrl, shortUrl);

            var cacheEntryOpts = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
            };

            await _cache.SetStringAsync(shortUrl.Id.ToString(), JsonSerializer.Serialize(shortUrl), cacheEntryOpts);

            return shortUrl;
        }

        public async Task Deactivate(int id)
        {
            ShortUrl? target = await _repo.GetOneById(id);
            if (target == null) throw new Exception("URL does not exist");
            await _repo.Deactivate(target);
            await _cache.SetStringAsync(id.ToString(), JsonSerializer.Serialize(target));
        }

        public async Task DeleteById(int id)
        {
            ShortUrl? target = await _repo.GetOneById(id);
            if (target == null) throw new Exception("URL does not exist");
            await _repo.DeleteOne(target);
            await _cache.RemoveAsync(id.ToString());
        }

        public async Task<List<ShortUrl>> GetAllShortUrlsForUser(int userId)
        {
            return await _repo.GetAllForUser(userId);
        }

        public async Task<ShortUrl> GetById(int id)
        {
            string? cahcedRes = await _cache.GetStringAsync(id.ToString());
            if (cahcedRes != null)
            {
                return JsonSerializer.Deserialize<ShortUrl>(cahcedRes)!;
            }

            ShortUrl? res = await _repo.GetOneById(id);
            if (res == null) throw new Exception("URL does not exist");

            var cacheEntryOpts = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
            };

            await _cache.SetStringAsync(res.Id.ToString(), JsonSerializer.Serialize(res), cacheEntryOpts);

            return res;
        }

        public async Task<ShortUrl> UpdateById(int id, ShortUrl newShortUrl)
        {
            ShortUrl? target = await _repo.GetOneById(id);
            if (target == null) throw new Exception("URL does not exist");
            ShortUrl res = await _repo.UpdateOne(target, newShortUrl);
            await _cache.SetStringAsync(id.ToString(), JsonSerializer.Serialize(res));
            return res;
        }
    }
}
