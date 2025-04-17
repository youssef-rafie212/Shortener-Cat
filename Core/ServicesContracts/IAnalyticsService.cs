﻿using Core.DTO.Analytics_DTOs;

namespace Core.ServicesContracts
{
    public interface IAnalyticsService
    {
        Task<MaxTotalVisitsForMultipleDto?> GetMaxVisitsForUserUrls(int userId);
        Task<MaxVisitsByCountryDto?> GetMaxVisitsByCountry(int urlId);
        Task<MaxVisitsByDeviceDto?> GetMaxVisitsByDevice(int urlId);
        Task<TotalVisitsDto?> GetVisitsForUrl(int urlId);
        Task<TotalVisitsForMultipleDto?> GetVisitsForUsersUrls(int userId);
        Task<VisitsByCountryDto?> GetVisitsByCountry(int urlId);
        Task<VisitsByDeviceDto?> GetVisitsByDevice(int urlId);
    }
}
