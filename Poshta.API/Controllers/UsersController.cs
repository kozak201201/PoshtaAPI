using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poshta.API.Contracts.Auth;
using Poshta.Application.Dtos;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;
using System.Security.Claims;

namespace Poshta.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(
        IUserService userService,
        IMapper mapper,
        ILogger<UsersController> logger) : ControllerBase
    {
        private readonly IUserService userService = userService;
        private readonly IMapper mapper = mapper;
        private readonly ILogger<UsersController> logger = logger;

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await userService.RegisterAsync(
            request.LastName,
            request.FirstName,
            request.Password,
            request.Phone,
            request.ConfirmationCode,
            request.MiddleName);

            if (result.IsFailure)
            {
                logger.LogWarning($"register user failed: {result.Error}");
                return BadRequest(new { result.Error });
            }

            logger.LogInformation($"New user register");

            return Ok(new { Message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            logger.LogInformation("Start Login");

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid model");
                return BadRequest(ModelState);
            }

            var tokenResult = await userService.LoginAsync(request.Phone, request.Password);

            if (tokenResult.IsFailure)
            {
                logger.LogError(tokenResult.Error);
                return BadRequest(tokenResult.Error);
            }

            var token = tokenResult.Value;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(60)
            };

            HttpContext?.Response.Cookies.Append("jwtToken", token, cookieOptions);

            logger.LogInformation("Finish Login");

            return Ok(new { token });
        }

        [Authorize]
        [HttpPut("update-email")]
        public async Task<IActionResult> UpdateEmail([FromBody] AddEmailRequest request)
        {
            logger.LogInformation("Start add email");

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid model");
                return BadRequest(ModelState.Values);
            }

            var userIdStr = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var userId))
            {
                logger.LogError("Invalid token");

                return Unauthorized(new { Error = "Invalid token" });
            }

            var result = await userService.UpdateEmailAsync(userId, request.Email, request.ConfirmationCode);

            if (result.IsFailure)
            {
                logger.LogWarning("Add email failed: {Error}", result.Error);

                return BadRequest(result.Error);
            }

            logger.LogInformation($"Email added successfully: {request.Email}");

            return Ok($"Email added successfully: {request.Email}");
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var tokenUserIdStr = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(tokenUserIdStr, out var tokenUserId))
            {
                logger.LogError($"Invalid token. user id: {userId}");
                return Unauthorized("Invalid token");
            }

            var userRoles = User?.Claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (userId != tokenUserId && !userRoles!.Contains("Admin") && !userRoles.Contains("Operator"))
            {
                logger.LogError($"Don't have permission to get user with id: {userId}");
                return Unauthorized($"Don't have permission to get user with id: {userId}");
            }

            var userResult = await userService.GetUserByIdAsync(userId);

            if (userResult.IsFailure)
            {
                return NotFound(userResult.Error);
            }

            var user = userResult.Value;

            var userDetails = mapper.Map<User, UserDetails>(user);

            return Ok(userDetails);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await userService.GetUsersAsync();

            var userDetails = mapper.Map<List<User>, List<UserDetails>>(users.ToList());

            return Ok(userDetails);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}/add-role")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleRequest request, Guid userId)
        {
            logger.LogInformation($"Start add role");

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid model");
                return BadRequest(ModelState.Values);
            }

            var role = request.Role;

            if (string.IsNullOrEmpty(role))
            {
                logger.LogError("Role can't be null or empty");
                return BadRequest("Role can't be null or empty");
            }

            var result = await userService.AddRoleAsync(userId, role);

            if (result.IsFailure)
                return BadRequest(result.Error);

            logger.LogInformation($"Success add role: {role} to user with id: {userId}");

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}/remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest request, Guid userId)
        {
            logger.LogInformation($"Start remove role");

            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid model");
                return BadRequest(ModelState.Values);
            }

            var role = request.Role;

            if (string.IsNullOrEmpty(role))
            {
                logger.LogError("Role can't be null or empty");
                return BadRequest("Role can't be null or empty");
            }

            var result = await userService.RemoveRoleAsync(userId, role);

            if (result.IsFailure)
                return BadRequest(result.Error);

            logger.LogInformation($"Success add role: {role} to user with id: {userId}");

            return Ok(result.Value);
        }

        [Authorize]
        [HttpDelete("{userId}/delete")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            logger.LogInformation($"Start delete user with id: {userId}");

            var tokenUserIdStr = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(tokenUserIdStr, out var tokenUserId))
            {
                logger.LogError($"Invalid token. user id: {userId}");
                return Unauthorized("Invalid token");
            }

            var userRoles = User?.Claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (!userRoles!.Contains("Admin") && userId != tokenUserId)
            {
                logger.LogError($"User with id: {tokenUserId} don't have permission to delete user with id: {userId}");
                return Forbid($"User with id: {tokenUserId} don't have permission to delete user with id: {userId}");
            }

            var result = await userService.DeleteUserAsync(userId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            logger.LogInformation($"Sser with id: {tokenUserId} success delete user with id: {userId}.");

            return Ok(result.Value);
        }
    }
}
