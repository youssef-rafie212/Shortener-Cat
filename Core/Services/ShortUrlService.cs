using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IShortUrlsRepo _repo;
        private readonly IShortenerService _shortenerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ShortUrlService(
            IShortUrlsRepo repo,
            IShortenerService shortenerService,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor
            )
        {
            _repo = repo;
            _shortenerService = shortenerService;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task Activate(int id)
        {
            ShortUrl? target = await _repo.GetOne(id);
            if (target == null) throw new Exception("Invalid ID");

            await _repo.Activate(target);
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

            shortUrl.Value = value;
            await _repo.UpdateOne(shortUrl, shortUrl);

            return shortUrl;
        }

        public async Task Deactivate(int id)
        {
            ShortUrl? target = await _repo.GetOne(id);
            if (target == null) throw new Exception("No URL with the given ID");
            await _repo.Deactivate(target);
        }

        public async Task DeleteById(int id)
        {
            ShortUrl? target = await _repo.GetOne(id);
            if (target == null) throw new Exception("No URL with the given ID");
            await _repo.DeleteOne(target);
        }

        public async Task<List<ShortUrl>> GetAllShortUrlsForUser(int userId)
        {
            return await _repo.GetAllForUser(userId);
        }

        public async Task<ShortUrl> GetById(int id)
        {
            ShortUrl? res = await _repo.GetOne(id);
            if (res == null) throw new Exception("No URL with the given ID");
            return res;
        }

        public async Task<ShortUrl> UpdateById(int id, ShortUrl newShortUrl)
        {
            ShortUrl? target = await _repo.GetOne(id);
            if (target == null) throw new Exception("No URL with the given ID");

            return await _repo.UpdateOne(target, newShortUrl);
        }
    }
}
