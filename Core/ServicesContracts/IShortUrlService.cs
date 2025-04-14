using Core.Domain.Entities;

namespace Core.ServicesContracts
{
    public interface IShortUrlService
    {
        Task<ShortUrl> CreateShortUrl(int userId, string originalUrl);
        Task<ShortUrl> GetById(int id);
        Task<List<ShortUrl>> GetAllShortUrlsForUser(int userId);
        Task DeleteById(int id);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<ShortUrl> UpdateById(int id, ShortUrl newShortUrl);
    }
}
