using CSharpFunctionalExtensions;
using Poshta.Application.Auth;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Poshta.Application.Services
{
    public class UserService(
        IUsersRepository usersRepository,
        IJwtProvider jwtProvider,
        IConfirmationCodeService confirmationCodeService,
        IServiceProvider serviceProvider,
        ILogger<UserService> logger) : IUserService
    {
        private readonly IUsersRepository usersRepository = usersRepository;
        private readonly IJwtProvider jwtProvider = jwtProvider;
        private readonly IConfirmationCodeService confirmationCodeService = confirmationCodeService;
        private readonly ILogger<UserService> logger = logger;

        public async Task<Result<string>> RegisterAsync(
            string lastName,
            string firstName,
            string password,
            string phoneNumber,
            string confirmationCode,
            string? middlename = null)
        {
            logger.LogInformation("Start registration user");
            var isCodeValid = await confirmationCodeService.ValidateCodeAsync(phoneNumber, confirmationCode);

            if (!isCodeValid)
            {
                logger.LogError("Invalid or expired code");
                return Result.Failure<string>("Invalid or expired code");
            }

            var userId = Guid.NewGuid();

            var userResult = User.Create(userId, lastName, firstName, password, phoneNumber, middlename);

            if (userResult.IsFailure)
            {
                logger.LogError(userResult.Error);
                return Result.Failure<string>(userResult.Error);
            }

            var user = userResult.Value;

            var result = await usersRepository.CreateAsync(user);

            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return Result.Failure<string>(result.Error);
            }

            confirmationCodeService.RemoveCode(phoneNumber);

            logger.LogInformation($"Success registration user. User id: {userId}");
            return Result.Success($"Success registration user. User id: {userId}");
        }

        public async Task<Result<string>> LoginAsync(string phoneNumber, string password)
        {
            logger.LogInformation("Start login user");

            var user = await usersRepository.GetByPhoneAsync(phoneNumber);

            if (user == null)
            {
                logger.LogError($"user with phone: {phoneNumber} wasn't found");
                return Result.Failure<string>($"user with phone: {phoneNumber} wasn't found");
            }

            bool result = await usersRepository.CheckPasswordAsync(user.Id, password);

            if (!result)
            {
                logger.LogError($"Invalid password");
                return Result.Failure<string>("Invalid password");
            }

            var userRoles = await usersRepository.GetRolesAsync(user.Id);

            var claims = new Dictionary<string, string>();

            if (userRoles.Contains("Operator"))
            {
                var operatorService = serviceProvider.GetService<IOperatorService>();
                var operatorResult = await operatorService!.GetByUserIdAsync(user.Id);

                if (operatorResult.IsSuccess)
                {
                    var operatorPostOffice = operatorResult.Value;

                    claims.Add("OperatorId", operatorPostOffice.Id.ToString());
                    claims.Add("PostOfficeId", operatorPostOffice.PostOfficeId.ToString());
                }
            }

            var token = jwtProvider.Generate(user.Id, userRoles.ToList(), claims);

            logger.LogInformation("Success login");
            return token;
        }

        public async Task<Result<string>> UpdateNameAsync(Guid id, string firstName, string lastName, string middleName)
        {
            logger.LogInformation($"Start update user name. User id: {id}");

            if (!Regex.IsMatch(lastName, User.NamePattern))
                return Result.Failure<string>("Invalid last name");

            if (!Regex.IsMatch(firstName, User.NamePattern))
                return Result.Failure<string>("Invalid first name");

            if (!string.IsNullOrEmpty(middleName) && !Regex.IsMatch(middleName, User.NamePattern))
                return Result.Failure<string>("Invalid middle name");

            await usersRepository.UpdateNameAsync(id, firstName, lastName, middleName);

            logger.LogInformation($"Success update user name. User id: {id}");
            return Result.Success($"Success update user name. User id: {id}");
        }

        public async Task<Result<User>> GetUserByPhoneAsync(string phone)
        {
            logger.LogInformation("Start get user by phone");

            var user = await usersRepository.GetByPhoneAsync(phone);

            if (user == null)
            {
                logger.LogError("User with this phone wasn't found");
                return Result.Failure<User>("User with this phone wasn't found");
            }

            logger.LogInformation("Success get user");
            return user;
        }

        public async Task<Result<User>> GetUserByIdAsync(Guid userId)
        {
            logger.LogInformation($"Start get user with id: {userId}");

            var user = await usersRepository.GetByIdAsync(userId);

            if (user == null)
            {
                logger.LogError("User with this phone wasn't found");
                return Result.Failure<User>("User with this phone wasn't found");
            }

            logger.LogInformation("Success get user");
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            logger.LogInformation($"Start get users");

            var users = await usersRepository.GetAllAsync();

            return users;
        }

        public async Task<Result<string>> UpdateEmailAsync(Guid userId, string email, string confirmationCode)
        {
            logger.LogInformation($"Start update email: {email} for the user with id: {userId}");

            var isCodeValid = await confirmationCodeService.ValidateCodeAsync(email, confirmationCode);

            if (!isCodeValid)
            {
                logger.LogError("Invalid or expired code");
                return Result.Failure<string>("Invalid or expired code");
            }

            await usersRepository.UpdateEmailAsync(userId, email);
            confirmationCodeService.RemoveCode(email);

            logger.LogInformation($"Success update email: {email} for user with id: {userId}");
            return Result.Success($"Success update email: {email} for user with id: {userId}");
        }

        public async Task<Result<string>> AddRoleAsync(Guid userId, string role)
        {
            logger.LogInformation($"Start add role: {role} to user with id: {userId}");

            var user = await usersRepository.GetByIdAsync(userId);

            if (user == null)
            {
                logger.LogError($"User with id: {userId} wasn't found");
                return Result.Failure<string>($"User with id: {userId} wasn't found");
            }

            var result = await usersRepository.AddRoleAsync(userId, role);

            if (result.IsFailure)
            {
                logger.LogError($"Failed to add role {role} from user: {userId}. {result.Error}");
                return Result.Failure<string>(result.Error);
            }

            logger.LogInformation($"role {role} add successfully to user: {userId}");
            return Result.Success($"role {role} add successfully to user: {userId}");
        }

        public async Task<Result<string>> RemoveRoleAsync(Guid userId, string role)
        {
            logger.LogInformation($"Start removing role: {role} from user with id: {userId}");

            var user = await usersRepository.GetByIdAsync(userId);

            if (user == null)
            {
                logger.LogError($"User with id: {userId} wasn't found");
                return Result.Failure<string>($"User with id: {userId} wasn't found");
            }

            var result = await usersRepository.RemoveRoleAsync(userId, role);

            if (result.IsFailure)
            {
                logger.LogError($"Failed to remove role {role} from user: {userId}. {result.Error}");
                return Result.Failure<string>(result.Error);
            }

            logger.LogInformation($"Role {role} removed successfully from user: {userId}");
            return Result.Success($"Role {role} removed successfully from user: {userId}");
        }

        public async Task<Result<string>> DeleteUserAsync(Guid userId)
        {
            logger.LogInformation($"Start deleting user with id: {userId}");

            var user = await usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                logger.LogError($"User with id: {userId} wasn't found");
                return Result.Failure<string>($"User with id: {userId} wasn't found");
            }

            var operatorService = serviceProvider.GetService<IOperatorService>();

            var operatorResult = await operatorService!.GetByUserIdAsync(userId);

            if (operatorResult.IsSuccess)
            {
                logger.LogError($"User with id: {userId} cannot be deleted because they are an operator");
                return Result.Failure<string>("User cannot be deleted because they are an operator");
            }

            var shipmentService = serviceProvider.GetService<IShipmentService>();

            var shipments = await shipmentService!.GetShipmentsByUserIdAsync(userId);

            if (shipments.Count() > 0)
            {
                logger.LogError($"User with id: {userId} cannot be deleted because they are involved in shipments");
                return Result.Failure<string>("User cannot be deleted because they are involved in shipments");
            }

            var result = await usersRepository.DeleteAsync(userId);

            if (result.IsFailure)
            {
                logger.LogError($"Failed to remove user: {userId}. {result.Error}");
                return Result.Failure<string>("Failed to remove user");
            }

            logger.LogInformation($"User with id: {userId} removed successfully");
            return Result.Success($"User with id: {userId} removed successfully");
        }
    }
}
