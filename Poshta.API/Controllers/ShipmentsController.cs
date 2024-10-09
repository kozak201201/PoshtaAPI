using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poshta.API.Contracts.Shipment;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;
using System.Security.Claims;

namespace Poshta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController(
        IShipmentService shipmentService,
        ILogger<ShipmentsController> logger) : ControllerBase
    {
        private readonly IShipmentService shipmentService = shipmentService;
        private readonly ILogger<ShipmentsController> logger = logger;

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request)
        {
            logger.LogInformation("Start create shipment");

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid model");
                return BadRequest(ModelState);
            }

            var userRoles = User.Claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var userId))
            {
                logger.LogError("Invalid token");
                return Unauthorized("Invalid token");
            }

            if (userId != request.SenderId && !userRoles.Contains("Employee"))
            {
                logger.LogError("Users can only create shipments for themselves");
                return Forbid("Users can only create shipments for themselves");
            }

            var result = await shipmentService.CreateShipmentAsync(
                request.SenderId,
                request.RecipientId,
                request.StartPostOfficeId,
                request.EndPostOfficeId,
                request.Payer,
                request.AppraisedValue ?? Shipment.MIN_APPRAISED_VALUE,
                request.Weight ?? Shipment.MIN_WEIGHT,
                request.Length ?? Shipment.MIN_LENGTH,
                request.Width ?? Shipment.MIN_WIDTH,
                request.Height ?? Shipment.MIN_HEIGHT);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation("Success create shipment");
            return Ok(result.Value);
        }

        [HttpGet("shipment-by-tracking-number")]
        public async Task<IActionResult> GetShipmentByTrackingNumber([FromQuery(Name = "trackingNumber")] string trackingNumber)
        {
            logger.LogInformation($"Start get shipment with tracking number: {trackingNumber}");

            var shipmentResult = await shipmentService.GetShipmentByTrackingNumberAsync(trackingNumber);

            if (shipmentResult.IsFailure)
            {
                logger.LogError(shipmentResult.Error);
                return BadRequest(shipmentResult.Error);
            }

            logger.LogInformation($"Success get shipment with tracking number: {trackingNumber}");
            return Ok(shipmentResult.Value);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("shipment-by-post-office-id")]
        public async Task<IActionResult> GetShipmentsByPostOfficeId([FromQuery(Name = "postOfficeId")] Guid postOfficeId)
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var userId))
            {
                logger.LogError("Invalid token");
                return Unauthorized("Invalid token");
            }
            
            logger.LogInformation($"Start get shipments with current post office id: {postOfficeId} " +
                $"by user with id: {userId}");

            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var operatorId = User.Claims.FirstOrDefault(c => c.Type == "OperatorId");

            if (roleClaim!.Value == "Operator")
            {
                var postOfficeIdClaim = User.Claims.FirstOrDefault(c => c.Type == "PostOfficeId");

                if (postOfficeIdClaim != null && Guid.TryParse(postOfficeIdClaim.Value, out var operatorPostOfficeId))
                {
                    if (operatorPostOfficeId != postOfficeId)
                    {
                        var errorMessage = "Operator with id: {operatorId} can't get shipments for " +
                            $"post office with id: {postOfficeId}. " +
                            $"Operator work at post office with id: {postOfficeIdClaim.Value}";

                        logger.LogError(errorMessage);
                        return Forbid(errorMessage);
                    }
                }
                else
                {
                    var errorMessage = $"Error with parse postOfficeIdClaim. " +
                        $"User with id: {userId} try to get shipments by post office with id: {postOfficeId}";

                    logger.LogError(errorMessage);
                    return Forbid(errorMessage);
                }
            }

            var shipments = await shipmentService.GetShipmentsByPostOfficeIdAsync(postOfficeId);

            logger.LogInformation($"Success get shipments with current post office id: {postOfficeId}");
            return Ok(shipments);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<IActionResult> GetAllShipments()
        {
            var adminIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(adminIdStr, out var adminId))
            {
                logger.LogError("Invalid token");
                return Unauthorized("Invalid token");
            }
            logger.LogInformation($"Start get all shipments. Admin user id: {adminId}");

            var shipments = await shipmentService.GetAllShipmentsAsync();

            logger.LogInformation($"Success get all shipments. Admin userId: {adminId}");
            return Ok(shipments);
        }

        [Authorize]
        [HttpGet("shipment-by-user-id")]
        public async Task<IActionResult> GetShipmentsByUserId([FromQuery(Name = "userId")] Guid userId)
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var tokenUserId))
            {
                logger.LogError("Invalid token");
                return Unauthorized("Invalid token");
            }

            logger.LogInformation($"Start get user shipments. User id: {userId}. " +
                $"User id who try to get: {tokenUserId}");

            if (userId != tokenUserId)
            {
                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                if (roleClaim == null)
                {
                    var errorMessage = $"Can't get role claim for user";

                    logger.LogError(errorMessage);
                    return Forbid(errorMessage);
                }

                if (roleClaim.Value != "Admin")
                {
                    var errorMessage = $"User with id: {tokenUserId} don't have permission " +
                        $"to get user with id: {userId} shipments";

                    logger.LogError(errorMessage);
                    return Forbid(errorMessage);
                }
            }

            var userShipments = await shipmentService.GetShipmentsByUserIdAsync(userId);

            logger.LogInformation($"User with id: {tokenUserId} success get user with id: {userId} shipments");
            return Ok(userShipments);
        }

        [Authorize(Roles = "Operator")]
        [HttpPut("{shipmentId}/accept")]
        public async Task<IActionResult> AcceptShipment(Guid shipmentId, [FromBody] AcceptShipmentRequest request)
        {
            logger.LogInformation($"Start accept shipment with id: {shipmentId}");

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid model");
                return BadRequest(ModelState);
            }

            var operatorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "OperatorId");

            if (operatorIdClaim == null)
            {
                logger.LogError($"Problem with operator claim");
                return Unauthorized("Problem with operator claim");
            }

            var operatorId = Guid.Parse(operatorIdClaim.Value);

            var operatorPostOfficeIdClaim = User.Claims.FirstOrDefault(c => c.Type == "PostOfficeId");

            if (operatorPostOfficeIdClaim == null)
            {
                logger.LogError($"Problem with operator claim");
                return Unauthorized("Problem with operator claim");
            }

            var operatorPostOfficeId = Guid.Parse(operatorPostOfficeIdClaim.Value);

            if (operatorPostOfficeId != request.PostOfficeId)
            {
                var errorMessage = $"Operator with post office id: {operatorPostOfficeId} " +
                    $"can't accept shipment with id: {shipmentId} in post office with id: {request.PostOfficeId}";
                logger.LogError(errorMessage);
                return Forbid(errorMessage);
            }

            var result = await shipmentService.AcceptShipmentAsync(shipmentId, request.PostOfficeId);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation(result.Value);
            return Ok(result.Value);
        }

        [Authorize(Roles = "Operator")]
        [HttpPut("{shipmentId}/send")]
        public async Task<IActionResult> SendShipment(Guid shipmentId)
        {
            logger.LogInformation($"Start send shipment with id: {shipmentId}");

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid model");
                return BadRequest(ModelState);
            }

            var operatorPostOfficeIdClaim = User.Claims.FirstOrDefault(c => c.Type == "PostOfficeId");

            if (operatorPostOfficeIdClaim == null)
            {
                logger.LogError($"Problem with operator post office id claim");
                return Unauthorized("Problem with operator post office id claim");
            }

            var operatorPostOfficeId = Guid.Parse(operatorPostOfficeIdClaim.Value);

            var result = await shipmentService.SendShipmentAsync(shipmentId, operatorPostOfficeId);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation(result.Value);
            return Ok(result.Value);
        }

        [HttpPut("{shipmentId}/redirect")]
        public async Task<IActionResult> RedirectShipment(Guid shipmentId, [FromBody] RedirectShipmentRequest request)
        {
            logger.LogInformation($"Start redirect shipment");

            if (!ModelState.IsValid)
            {
                logger.LogError(ModelState.ToString());
                return BadRequest(ModelState);
            }

            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var userId))
            {
                logger.LogError("Invalid token");
                return Unauthorized("Invalid token");
            }

            var result = await shipmentService.RedirectShipmentAsync(userId, shipmentId, request.NewEndPostOfficeId);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation($"Success redirect shipment with id: {shipmentId} to " +
                $"new post office with id: {request.NewEndPostOfficeId}");

            return Ok(result.Value);
        }

        [Authorize(Roles = "Operator")]
        [HttpPut("{shipmentId}/receive")]
        public async Task<IActionResult> ReceiveShipment(Guid shipmentId, [FromBody] ReceiveShipmentRequest request)
        {
            logger.LogInformation($"Start receive shipment");

            if (!ModelState.IsValid)
            {
                logger.LogError(ModelState.ToString());
                return BadRequest(ModelState);
            }

            var operatorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "OperatorId");

            if (operatorIdClaim == null)
            {
                logger.LogError($"Problem with operator claim");
                return Unauthorized("Problem with operator claim");
            }

            var operatorId = Guid.Parse(operatorIdClaim.Value);

            var result = await shipmentService.ReceiveShipmentAsync(request.RecipientId, shipmentId, operatorId);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation(result.Value);

            return Ok(new { message = result.Value, operatorId });
        }

        [Authorize]
        [HttpDelete("{shipmentId}/delete")]
        public async Task<IActionResult> DeleteShipment(Guid shipmentId)
        {
            logger.LogInformation($"Start delete shipment");

            if (!ModelState.IsValid)
            {
                logger.LogError(ModelState.ToString());
                return BadRequest(ModelState);
            }

            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var userId))
            {
                logger.LogError("Invalid token");
                return Unauthorized("Invalid token");
            }

            var result = await shipmentService.SoftDeleteShipmentAsync(shipmentId, userId);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation($"Success delete shipment with id: {shipmentId}");
            return Ok(result.Value);
        }
    }
}
