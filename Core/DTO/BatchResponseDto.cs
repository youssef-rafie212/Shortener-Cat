namespace Core.DTO
{
    public class BatchResponseDto
    {
        // Device tokens that the notification was sent to them.
        public List<string> SuccessfulDevices { get; set; } = [];

        // Device tokens that the notification was not sent to them.
        public List<string> FailedDevices { get; set; } = [];
    }
}
