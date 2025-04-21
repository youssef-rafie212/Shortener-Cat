using Core.DTO;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Mvc;

namespace Shortener_Cat.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/push-notifications")]
    [ApiVersion("1.0")]
    public class PushNotificationsController : ControllerBase
    {
        private readonly IPushNotificationService _notificationService;

        public PushNotificationsController(IPushNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        [Route("register-device")]
        public async Task<IActionResult> Register(RegisterDeviceDto dto)
        {
            try
            {
                await _notificationService.RegisterDevice(dto.UserId, dto.DeviceToken);
                return Ok("Device Registered");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("unregister-device")]
        public async Task<IActionResult> Unregister(UnregisterDevice dto)
        {
            try
            {
                await _notificationService.UnregisterDevice(dto.DeviceToken);
                return Ok("Device unregistered");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("push-for-one")]
        public async Task<IActionResult> PushForOne(PushNotiForOneDto dto)
        {
            try
            {
                await _notificationService.PushNotificationForOne(dto.DeviceToken, dto.Title, dto.Body);
                return Ok("Notification sent");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("push-for-multiple")]
        public async Task<IActionResult> PushForMultiple(PushNotiForMultipleDto dto)
        {
            try
            {
                var res = await _notificationService.PushNotificationForMultiple(dto.DeviceTokens, dto.Title, dto.Body);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("push-for-all")]
        public async Task<IActionResult> PushForAll(PushNotiForAllDto dto)
        {
            try
            {
                var res = await _notificationService.PushNotificationForAll(dto.Title, dto.Body);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
