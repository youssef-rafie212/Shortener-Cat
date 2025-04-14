using Core.Domain.Entities;

namespace Core.Domain.RepositoryContracts
{
    public interface IShortUrlsRepo
    {
        Task AddOne(ShortUrl shortUrl);
        Task<ShortUrl?> GetOne(int id);
        Task DeleteOne(ShortUrl shortUrl);
        Task<ShortUrl> UpdateOne(ShortUrl oldShortUrl, ShortUrl newShortUrl);
        Task<List<ShortUrl>> GetAllForUser(int userId);
        Task Activate(ShortUrl shortUrl);
        Task Deactivate(ShortUrl shortUrl);
    }
}
