using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poshta.API.Contracts.PostOffice;
using Poshta.Core.Interfaces.Services;
using System.Security.Claims;

namespace Poshta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostOfficesController(
        IPostOfficeService postOfficeService,
        ILogger<PostOfficesController> logger) : ControllerBase
    {
        private readonly IPostOfficeService postOfficeService = postOfficeService;
        private readonly ILogger<PostOfficesController> logger = logger;

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePostOffice([FromBody] CreatePostOfficeRequest request)
        {
            logger.LogInformation($"Start create post office");

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

            var result = await postOfficeService.CreateAsync(
                request.Number,
                request.City,
                request.Address,
                request.MaxShipmentsCount,
                request.Latitude,
                request.Longitude,
                request.PostOfficeTypeId);

            if (result.IsFailure)
            {
                logger.LogError($"Fail result post office service create\n\r{result.Error}");
                return BadRequest(result.Error);
            }

            logger.LogInformation($"Success create post office by admin with user id: {adminId}. " +
                $"Post office id: {result.Value.Id}");
            return Ok($"Success create post office by admin with user id: {adminId}. " +
                $"Post office id: {result.Value.Id}");
        }

        [HttpGet("{postOfficeId}")]
        public async Task<IActionResult> GetPostOffice(Guid postOfficeId)
        {
            logger.LogInformation($"Start get post office: {postOfficeId}");

            var postOfficeResult = await postOfficeService.GetPostOfficeByIdAsync(postOfficeId);

            if (postOfficeResult.IsFailure)
            {
                logger.LogError(postOfficeResult.Error);
                return BadRequest(postOfficeResult.Error);
            }

            var postOffice = postOfficeResult.Value;

            logger.LogInformation($"Success get post office with id: {postOfficeId}");
            return Ok(postOffice);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPostOffices()
        {
            logger.LogInformation($"Start get post offices");
            var postOffices = await postOfficeService.GetPostOfficesAsync();

            logger.LogInformation($"Success get post offices");
            return Ok(postOffices);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{postOfficeId}")]
        public async Task<IActionResult> DeletePostOffice(Guid postOfficeId)
        {
            logger.LogInformation($"Start delete post office with id: {postOfficeId}");
            
            var result = await postOfficeService.DeleteAsync(postOfficeId);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return BadRequest(result.Error);
            }

            logger.LogInformation($"Success delete post office with id: {postOfficeId}");
            return Ok($"Success delete post office with id: {postOfficeId}");
        }
    }
}
