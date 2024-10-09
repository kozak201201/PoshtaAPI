using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;

namespace Poshta.Application.Services
{
    public class OperatorService(
        IOperatorsRepository operatorsRepository,
        IUserService userService,
        IServiceProvider serviceProvider,
        ILogger<OperatorService> logger) : IOperatorService
    {
        private readonly IOperatorsRepository operatorRepository = operatorsRepository;
        private readonly IUserService userService = userService;
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private readonly ILogger<OperatorService> logger = logger;

        public async Task<Result<Operator>> CreateAsync(Guid userId, Guid postOfficeId)
        {
            logger.LogInformation($"Start create operator. UserId: {userId}. PostOfficeId: {postOfficeId}");

            var userResult = await userService.GetUserByIdAsync(userId);

            if (userResult.IsFailure)
            {
                logger.LogError(userResult.Error);
                return Result.Failure<Operator>(userResult.Error);
            }

            var operatorPostOfficeResult = Operator.Create(Guid.NewGuid(), userId, postOfficeId);

            if(operatorPostOfficeResult.IsFailure)
            {
                logger.LogError(operatorPostOfficeResult.Error);
                return Result.Failure<Operator>(operatorPostOfficeResult.Error);
            }

            var operatorPostOffice = operatorPostOfficeResult.Value;

            await operatorRepository.CreateAsync(operatorPostOffice);

            logger.LogInformation($"Success create operator with id: {operatorPostOffice.Id}. " +
                $"UserId: {userId}. PostOfficeId: {postOfficeId}.");

            return Result.Success(operatorPostOffice);
        }

        public async Task<Result<Operator>> GetByIdAsync(Guid operatorId)
        {
            logger.LogInformation($"Start get operator with id: {operatorId}");

            var operatorPostOffice = await operatorRepository.GetByIdAsync(operatorId);

            if (operatorPostOffice == null)
            {
                logger.LogError($"Operator with id: {operatorId} wasn't found");
                return Result.Failure<Operator>($"Operator with id: {operatorId} wasn't found");
            }

            logger.LogInformation($"Success get operator with id: {operatorId}");
            return Result.Success(operatorPostOffice);
        }

        public async Task<Result<Operator>> GetByUserIdAsync(Guid userId)
        {
            logger.LogInformation($"Start get operator with user id: {userId}");

            var operatorPostOffice = await operatorRepository.GetByUserIdAsync(userId);

            if (operatorPostOffice == null)
            {
                logger.LogError($"Operator with user id: {userId} wasn't found");
                return Result.Failure<Operator>($"Operator with user id: {userId} wasn't found");
            }

            logger.LogInformation($"Success get operator with user id: {userId}");
            return Result.Success(operatorPostOffice);
        }

        public async Task<IEnumerable<Operator>> GetAllAsync()
        {
            logger.LogInformation($"Start get all operators");

            var operators = await operatorRepository.GetAllAsync();

            logger.LogInformation($"Success get all operators");
            return operators;
        }

        public async Task<Result<string>> UpdatePostOfficeAsync(Guid operatorId, Guid postOfficeId)
        {
            logger.LogInformation($"Start update operator post office. " +
                $"operator id: {operatorId}. post office id: {postOfficeId}");

            var operatorPostOffice = await operatorRepository.GetByIdAsync(operatorId);

            if (operatorPostOffice == null)
            {
                logger.LogError($"Operator with id: {operatorId} wasn't found");
                return Result.Failure<string>($"Operator with id: {operatorId} wasn't found");
            }

            await operatorRepository.UpdatePostOfficeAsync(operatorId, postOfficeId);

            logger.LogInformation($"Success update operator post office. " +
                $"operator id: {operatorId}. post office id: {postOfficeId}.");

            return Result.Success($"Success update operator post office. " +
                $"operator id: {operatorId}. post office id: {postOfficeId}.");
        }

        public async Task<Result<string>> AddRatingAsync(
            Guid operatorId,
            Guid userId,
            Guid shipmentId,
            int rating,
            string review)
        {
            logger.LogInformation($"Start add rating for operator with id: {operatorId}." +
                $"From user with id: {userId} relative to shipment with id: {shipmentId}");

            var shipmentService = serviceProvider.GetService<IShipmentService>();

            var shipmentResult = await shipmentService!.GetShipmentByIdAsync(shipmentId);

            if (shipmentResult.IsFailure)
            {
                logger.LogError(shipmentResult.Error);
                return Result.Failure<string>(shipmentResult.Error);
            }

            var shipment = shipmentResult.Value;

            if (shipment.Status != ShipmentStatus.Delivered)
            {
                logger.LogError($"User with id: {userId} can't rate operator with id: {operatorId} " +
                    $"because shipment with id: {shipmentId} doesn't have status: Delivered");

                return Result.Failure<string>($"User with id: {userId} can't rate operator  " +
                    $"because shipment doesn't have status: Delivered");
            }

            if (!shipment.OperatorWhoIssuedId.HasValue)
            {
                logger.LogError($"Operator who issued shipment with id: {shipmentId} wasn't found");
                return Result.Failure<string>($"Operator who issued shipment with id: {shipmentId} wasn't found");
            }

            var operatorResult = await GetByIdAsync(operatorId);

            if (operatorResult.IsFailure)
            {
                logger.LogError(operatorResult.Error);
                return Result.Failure<string>(operatorResult.Error);
            }

            var operatorPostOffice = operatorResult.Value;

            var userResult = await userService.GetUserByIdAsync(userId);

            if (userResult.IsFailure)
            {
                logger.LogError(userResult.Error);
                return Result.Failure<string>(userResult.Error);
            }

            var user = userResult.Value;

            if (shipment.OperatorWhoIssuedId != operatorPostOffice.Id)
            {
                logger.LogError($"User with id: {user.Id} can't rate " +
                    $"operator with id: {operatorId} because operator did not transfer " +
                    $"the shipment with id: {shipment} to the recipient");

                return Result.Failure<string>($"User with id: {user.Id} can't rate " +
                    $"operator with id: {operatorId} because operator did not transfer " +
                    $"the shipment with id: {shipment.Id} to the recipient");
            }

            if (shipment.RecipientId != user.Id && shipment.ConfidantId != userId)
            {
                logger.LogError($"User with id: {user.Id} isn'n confidant or recipient " +
                    $"that's why he can't rate operator with id: {operatorId} ralative to " +
                    $"shipment with id: {shipment.Id}");

                return Result.Failure<string>($"User with id: {user.Id} isn'n confidant or recipient " +
                    $"that's why he can't rate operator");
            }

            if (operatorId == userId)
            {
                logger.LogError($"Fail. Operator with id: {operatorId} try to rate himself");
                return Result.Failure<string>($"Fail. Operator with id: {operatorId} try to rate himself");
            }

            var createdAt = DateTime.Now;

            var operatorRatingResult = OperatorRating.Create(
                Guid.NewGuid(),
                operatorId,
                userId,
                rating,
                review,
                createdAt);

            if (operatorRatingResult.IsFailure)
            {
                logger.LogError(operatorRatingResult.Error);
                return Result.Failure<string>(operatorRatingResult.Error);
            }

            var operatorRating = operatorRatingResult.Value;

            await operatorRepository.AddRatingAsync(operatorRating);

            logger.LogInformation($"Success add rating for operator with id: {operatorId}." +
                $"From user with id: {userId}. " +
                $"Shipment id: {shipment.Id}");

            return Result.Success($"Success add rating for operator with id: {operatorId}." +
                $"From user with id: {userId}. " +
                $"Shipment id: {shipment.Id}");
        }

        public async Task<Result<string>> DeleteAsync(Guid operatorId)
        {
            logger.LogInformation($"Start delete operator with id: {operatorId}.");

            var operatorPostOffice = await operatorRepository.GetByIdAsync(operatorId);

            if (operatorPostOffice == null)
            {
                logger.LogError($"Operator with id: {operatorId} wasn't found");
                return Result.Failure<string>($"Operator with id: {operatorId} wasn't found");
            }

            await operatorRepository.DeleteAsync(operatorId);

            logger.LogInformation($"Success delete operator with id: {operatorId}.");
            return Result.Success($"Success delete operator with id: {operatorId}.");
        }
    }
}
