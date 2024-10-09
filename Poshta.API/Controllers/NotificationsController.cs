using Microsoft.AspNetCore.Mvc;
using Poshta.API.Contracts.Auth;
using Poshta.Application.Interfaces.Services;

namespace Poshta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger) : ControllerBase
    {
        [HttpPost("sms-send-code")]
        public async Task<IActionResult> SmsSendConfirmationCode([FromBody] SmsSendCodeRequest request)
        {
            logger.LogInformation("Start send confirmation code sms");

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid model");
                return BadRequest(ModelState.Values);
            }

            var result = await notificationService.SmsSendConfirmationCodeAsync(request.Phone, true);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation(result.Value);
            return Ok($"Code sent to {request.Phone}. Please enter the code to complete the process.");
        }

        [HttpPost("email-send-code")]
        public async Task<IActionResult> EmailSendConfirmationCode([FromBody] EmailSendCodeRequest request)
        {
            logger.LogInformation("Start send confirmation code email");

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid model");
                return BadRequest(ModelState.Values);
            }

            var result = await notificationService.EmailSendConfirmationCodeAsync(request.Email);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation(result.Value);

            return Ok($"Code sent to {request.Email}. " +
                $"Please enter the code to complete the process.");
        }
    }
}
