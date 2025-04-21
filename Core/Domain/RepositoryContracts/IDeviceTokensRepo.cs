using Core.Domain.Entities;

namespace Core.Domain.RepositoryContracts
{
    public interface IDeviceTokensRepo
    {
        Task AddOne(DeviceToken deviceToken);
        Task<DeviceToken?> GetOneById(int tokenId);
        Task<DeviceToken?> GetOneByUserId(int userId);
        Task<DeviceToken?> GetOneByToken(string deviceToken);
        Task<List<DeviceToken>> GetAll();
        Task RemoveOne(DeviceToken deviceToken);
    }
}
