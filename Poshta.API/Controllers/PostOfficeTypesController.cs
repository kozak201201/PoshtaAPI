using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poshta.API.Contracts.PostOfficeType;
using Poshta.Core.Interfaces.Services;

namespace Poshta.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PostOfficeTypesController(
        IPostOfficeTypeService postOfficeTypeService,
        ILogger<PostOfficeTypesController> logger) : ControllerBase
    {
        private readonly IPostOfficeTypeService postOfficeTypeService = postOfficeTypeService;
        private readonly ILogger<PostOfficeTypesController> logger = logger;
        
        [HttpPost("create")]
        public async Task<IActionResult> CreatePostOfficeType([FromBody] CreatePostOfficeTypeRequest request)
        {
            logger.LogInformation($"Start create post office type");

            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid model");
                return BadRequest(ModelState);
            }

            var postOfficeTypeResult = await postOfficeTypeService.CreateAsync(
                request.Name,
                request.MaxShipmentWeight,
                request.MaxShipmentLength,
                request.MaxShipmentWidtht,
                request.MaxShipmentHeight);

            if (postOfficeTypeResult.IsFailure)
            {
                logger.LogError(postOfficeTypeResult.Error);
                return BadRequest(postOfficeTypeResult.Error);
            }

            logger.LogInformation($"Success create post office type. PostOfficeTypeId: {postOfficeTypeResult.Value.Id}");
            return Ok(postOfficeTypeResult.Value);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPostOfficeTypes()
        {
            logger.LogInformation($"Start get all post office types");

            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid model");
                return BadRequest(ModelState);
            }

            var postOfficeTypes = await postOfficeTypeService.GetAllAsync();

            logger.LogInformation($"Success get all post office types");
            return Ok(postOfficeTypes);
        }

        [HttpGet("{postOfficeTypeId}")]
        public async Task<IActionResult> GetPostOfficeTypeById(Guid postOfficeTypeId)
        {
            logger.LogInformation($"Start get post office type with id: {postOfficeTypeId}");

            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid model");
                return BadRequest(ModelState);
            }

            var postOfficeTypeResult = await postOfficeTypeService.GetByIdAsync(postOfficeTypeId);

            if (postOfficeTypeResult.IsFailure)
            {
                logger.LogError(postOfficeTypeResult.Error);
                return BadRequest(postOfficeTypeResult.Error);
            }

            logger.LogInformation($"Success get post office type with id: {postOfficeTypeId}");
            return Ok(postOfficeTypeResult.Value);
        }

        [HttpDelete("{postOfficeTypeId}")]
        public async Task<IActionResult> DeletePostOfficeType(Guid postOfficeTypeId)
        {
            logger.LogInformation($"Start delete post office type with id: {postOfficeTypeId}");

            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid model");
                return BadRequest(ModelState);
            }

            var postOfficeTypeResult = await postOfficeTypeService.DeleteByIdAsync(postOfficeTypeId);

            if (postOfficeTypeResult.IsFailure)
            {
                logger.LogError(postOfficeTypeResult.Error);
                return BadRequest(postOfficeTypeResult.Error);
            }

            logger.LogInformation($"Success delete post office type with id: {postOfficeTypeId}");
            return Ok(postOfficeTypeResult.Value);
        }
    }
}
