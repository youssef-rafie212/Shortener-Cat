using Core.Domain.Entities;

namespace Core.ServicesContracts
{
    public interface IUrlVisitService
    {
        Task AddOne(UrlVisit visit);
        Task<UrlVisit> GetOneById(int id);
        Task<List<UrlVisit>> GetAllVisitsForUrl(int shortUrlId);
    }
}
