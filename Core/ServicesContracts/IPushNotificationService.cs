using Core.DTO;

namespace Core.ServicesContracts
{
    public interface IPushNotificationService
    {
        Task RegisterDevice(int userId, string deviceToken);
        Task UnregisterDevice(string deviceToken);
        Task PushNotificationForOne(string deviceToken, string title, string body);
        Task<BatchResponseDto> PushNotificationForMultiple(List<string> deviceTokens, string title, string body);
        Task<BatchResponseDto> PushNotificationForAll(string title, string body);
    }
}
