﻿using Core.DTO.Analytics_DTOs;

namespace Core.ServicesContracts
{
    public interface IAnalyticsService
    {
        Task<MaxTotalVisitsForMultipleDto?> GetMaxVisitsForUserUrls(int userId);
        Task<MaxVisitsByCountryDto?> GetMaxVisitsByCountry(int urlId);
        Task<MaxVisitsByDeviceDto?> GetMaxVisitsByDevice(int urlId);
        Task<TotalVisitsDto> GetVisitsForUrl(int urlId);
        Task<List<VisitsByCountryDto>> GetVisitsByCountry(int urlId);
        Task<List<VisitsByDeviceDto>> GetVisitsByDevice(int urlId);
    }
}
