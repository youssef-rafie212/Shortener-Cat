using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.ServicesContracts;

namespace Core.Services
{
    public class UrlVisitService : IUrlVisitService
    {
        private readonly IUrlVisitsRepo _repo;

        public UrlVisitService(IUrlVisitsRepo repo)
        {
            _repo = repo;
        }

        public async Task AddOne(UrlVisit visit)
        {
            await _repo.AddOne(visit);
        }

        public async Task<List<UrlVisit>> GetAllVisitsForUrl(int shortUrlId)
        {
            return await _repo.GetAllVisitsForUrl(shortUrlId);
        }

        public async Task<UrlVisit> GetOneById(int id)
        {
            UrlVisit? visit = await _repo.GetOneById(id);
            if (visit == null) throw new Exception("No URL visit with this ID");
            return visit;
        }
    }
}
