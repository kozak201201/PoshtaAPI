using Microsoft.AspNetCore.Mvc;
using Poshta.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Poshta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentController> logger;

        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger)
        {
            this.paymentService = paymentService;
            this.logger = logger;
        }

        [Authorize]
        [HttpGet("create-payment-request")]
        public async Task<IActionResult> CreatePaymentRequest([FromQuery] Guid shipmentId)
        {
            logger.LogInformation($"Create payment request: {shipmentId}");

            var userIdStr = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var userId))
            {
                logger.LogError("Invalid token");

                return Unauthorized(new { Error = "Invalid token" });
            }

            var paymentUrlResult = await paymentService.CreatePaymentRequestUrlAsync(shipmentId, userId);

            if (paymentUrlResult.IsFailure)
            {
                logger.LogError(paymentUrlResult.Error);
                return BadRequest(paymentUrlResult.Error);
            }

            var paymentUrl = paymentUrlResult.Value;

            if (string.IsNullOrEmpty(paymentUrl))
            {
                logger.LogError($"Failed to create payment request for shipmentId: {shipmentId}");
                return BadRequest("Unable to generate payment URL.");
            }

            logger.LogInformation($"Success payment request: {shipmentId}");
            return Ok(new { paymentUrl });
        }

        [HttpPost("process-payment-result")]
        public async Task<IActionResult> ProcessPaymentResult([FromForm] IFormCollection form)
        {
            logger.LogInformation($"Process payment result");

            var data = form["data"].ToString();
            var signature = form["signature"].ToString();

            var parameters = new Dictionary<string, string>
            {
                { "data", data },
                { "signature", signature }
            };

            var result = await paymentService.ProcessPaymentResult(parameters);

            if (result.IsFailure)
            {
                logger.LogError($"Process payment result success");
                return BadRequest(new { result.Error });
            }

            logger.LogInformation($"Process payment result success");
            return Ok(new { result.Value });
        }
    }
}
