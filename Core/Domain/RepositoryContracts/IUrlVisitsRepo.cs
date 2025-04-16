using Core.Domain.Entities;

namespace Core.Domain.RepositoryContracts
{
    public interface IUrlVisitsRepo
    {
        Task AddOne(UrlVisit visit);
        Task<UrlVisit?> GetOneById(int id);
        Task<List<UrlVisit>> GetAllVisitsForUrl(int shortUrlId);
    }
}
