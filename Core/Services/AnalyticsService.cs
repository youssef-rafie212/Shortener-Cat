using Core.Domain.RepositoryContracts;
using Core.DTO.Analytics_DTOs;
using Core.ServicesContracts;

namespace Core.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepo _repo;

        public AnalyticsService(IAnalyticsRepo repo)
        {
            _repo = repo;
        }

        public async Task<MaxVisitsByCountryDto?> GetMaxVisitsByCountry(int urlId)
        {
            return await _repo.GetMaxVisitsByCountry(urlId);
        }

        public async Task<MaxVisitsByDeviceDto?> GetMaxVisitsByDevice(int urlId)
        {
            return await _repo.GetMaxVisitsByDevice(urlId);
        }

        // Get url with max visits (ik the name sucks)
        public async Task<MaxTotalVisitsForMultipleDto?> GetMaxVisitsForUserUrls(int userId)
        {
            return await _repo.GetMaxVisitsForUserUrls(userId);
        }

        public async Task<List<VisitsByCountryDto>> GetVisitsByCountry(int urlId)
        {
            return await _repo.GetVisitsByCountry(urlId);
        }

        public async Task<List<VisitsByDeviceDto>> GetVisitsByDevice(int urlId)
        {
            return await _repo.GetVisitsByDevice(urlId);
        }

        public async Task<TotalVisitsDto> GetVisitsForUrl(int urlId)
        {
            return await _repo.GetVisitsForUrl(urlId);
        }
    }
}
