using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poshta.API.Contracts.Operator;
using Poshta.Core.Interfaces.Services;
using System.Security.Claims;

namespace Poshta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorsController(
        IOperatorService operatorService,
        ILogger<OperatorsController> logger) : ControllerBase
    {
        private readonly IOperatorService operatorService = operatorService;
        private readonly ILogger<OperatorsController> logger = logger;

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOperatorRequest request)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid model");
                return BadRequest(ModelState);
            }

            var adminIdClaim = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (adminIdClaim == null)
            {
                logger.LogError($"Problem with admin claim");
                return Unauthorized("Problem with admin claim");
            }

            var adminId = Guid.Parse(adminIdClaim.Value);

            logger.LogInformation($"Start create operator by admin with user id: {adminId}");
            
            var result = await operatorService.CreateAsync(request.UserId, request.PostOfficeId);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation($"Success create operator: Operator Id: {result.Value.Id}. " +
                $"Admin user id: {adminId}");
            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            logger.LogInformation($"Start get operators");
            var operators = await operatorService.GetAllAsync();

            logger.LogInformation($"Success get operators");
            return Ok(operators);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{operatorId}")]
        public async Task<IActionResult> GetById(Guid operatorId)
        {
            logger.LogInformation($"Start get operators");
            var operatorResult = await operatorService.GetByIdAsync(operatorId);

            if (operatorResult.IsFailure)
            {
                logger.LogError(operatorResult.Error);
                return BadRequest(operatorResult.Error);
            }

            var operatorPostOffice = operatorResult.Value;

            logger.LogInformation($"Success get operator with id: {operatorId}");
            return Ok(operatorPostOffice);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{operatorId}/updatePostOffice")]
        public async Task<IActionResult> UpdatePostOffice(Guid operatorId, [FromBody] UpdateOperatorPostOfficeRequest request)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid model");
                return BadRequest(ModelState);
            }

            var adminIdClaim = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (adminIdClaim == null)
            {
                logger.LogError($"Problem with admin claim");
                return Unauthorized("Problem with admin claim");
            }
            var adminId = Guid.Parse(adminIdClaim.Value);

            logger.LogInformation($"Start update operator post office. " +
                $"Operator id: {operatorId}. New post office id: {request.NewPostOfficeId}. " +
                $"Admin user id: {adminId}");

            var updateOperatorPostOfficeResult = await operatorService.UpdatePostOfficeAsync(operatorId, request.NewPostOfficeId);

            if (updateOperatorPostOfficeResult.IsFailure)
            {
                logger.LogError(updateOperatorPostOfficeResult.Error);
                return BadRequest(updateOperatorPostOfficeResult.Error);
            }

            logger.LogInformation(updateOperatorPostOfficeResult.Value);
            return Ok(updateOperatorPostOfficeResult.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{operatorId}/delete")]
        public async Task<IActionResult> Delete(Guid operatorId, [FromBody] DeleteOperatorRequest request)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid model");
                return BadRequest(ModelState);
            }

            var adminIdClaim = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (adminIdClaim == null)
            {
                logger.LogError($"Problem with user claim");
                return Unauthorized("Problem with user claim");
            }
            var adminId = Guid.Parse(adminIdClaim.Value);

            logger.LogInformation($"Start delete operator with id: {operatorId} by " +
                $"Admin with user id: {adminId}");

            var deleteOperatorPostOfficeResult = await operatorService.DeleteAsync(operatorId);

            if (deleteOperatorPostOfficeResult.IsFailure)
            {
                logger.LogError(deleteOperatorPostOfficeResult.Error);
                return BadRequest(deleteOperatorPostOfficeResult.Error);
            }

            logger.LogInformation(deleteOperatorPostOfficeResult.Value);
            return Ok(deleteOperatorPostOfficeResult.Value);
        }

        [HttpPut("{operatorId}/addRating")]
        public async Task<IActionResult> AddRating(Guid operatorId, [FromBody] AddRatingRequest request)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid model");
                return BadRequest(ModelState);
            }

            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                logger.LogError($"Problem with user claim");
                return Unauthorized("Problem with user claim");
            }
            var userId = Guid.Parse(userIdClaim.Value);

            logger.LogInformation($"Start add rating to operator with id: {operatorId} by " +
                $"User with id: {userId}");

            var addRatingOperatorResult = await operatorService.AddRatingAsync(
                operatorId, 
                userId,
                request.ShipmentId,
                request.Rating, 
                request.Review);

            if (addRatingOperatorResult.IsFailure)
            {
                logger.LogError(addRatingOperatorResult.Error);
                return BadRequest(addRatingOperatorResult.Error);
            }

            logger.LogInformation(addRatingOperatorResult.Value);
            return Ok(addRatingOperatorResult.Value);
        }
    }
}
