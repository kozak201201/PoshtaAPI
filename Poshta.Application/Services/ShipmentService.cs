using CSharpFunctionalExtensions;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;
using Microsoft.Extensions.Logging;
using Poshta.Application.Interfaces.Services;

namespace Poshta.Application.Services
{
    public class ShipmentService(
        IShipmentsRepository shipmentsRepository,
        IUserService userService,
        IOperatorService operatorService,
        IPostOfficeService postOfficeService,
        INotificationService notificationService,
        ILogger<ShipmentService> logger) : IShipmentService
    {
        private readonly IShipmentsRepository shipmentsRepository = shipmentsRepository;
        private readonly IPostOfficeService postOfficeService = postOfficeService;
        private readonly IUserService userService = userService;
        private readonly IOperatorService operatorService = operatorService;
        private readonly INotificationService notificationService = notificationService;
        private readonly ILogger<ShipmentService> logger = logger;

        public async Task<Result<Shipment>> CreateShipmentAsync(
            Guid senderId,
            Guid recipientId,
            Guid startPostOfficeId,
            Guid endPostOfficeId,
            PayerType payer,
            double appraisedValue = Shipment.MIN_APPRAISED_VALUE,
            float weight = Shipment.DEFAULT_WEIGHT,
            float length = Shipment.MIN_LENGTH,
            float width = Shipment.MIN_WIDTH,
            float height = Shipment.MIN_HEIGHT)
        {
            logger.LogInformation($"Start create shipment");

            var startPostOfficeResult = await postOfficeService.GetPostOfficeByIdAsync(startPostOfficeId);

            if (startPostOfficeResult.IsFailure)
            {
                logger.LogError(startPostOfficeResult.Error);
                return Result.Failure<Shipment>(startPostOfficeResult.Error);
            }

            var startPostOffice = startPostOfficeResult.Value;

            var startPostOfficeValidationResult = ValidationShipmentForPostOffice(
                    weight, length, width, height,
                    startPostOffice.Type.MaxShipmentWeight,
                    startPostOffice.Type.MaxShipmentLength,
                    startPostOffice.Type.MaxShipmentWidth,
                    startPostOffice.Type.MaxShipmentHeight);

            if (startPostOfficeValidationResult.IsFailure)
            {
                logger.LogError(startPostOfficeValidationResult.Error);
                return Result.Failure<Shipment>(startPostOfficeValidationResult.Error);
            }

            var endPostOfficeResult = await postOfficeService.GetPostOfficeByIdAsync(endPostOfficeId);

            if (endPostOfficeResult.IsFailure)
            {
                logger.LogError(endPostOfficeResult.Error);
                return Result.Failure<Shipment>(endPostOfficeResult.Error);
            }

            var endPostOffice = endPostOfficeResult.Value;

            var endPostOfficeValidationResult = ValidationShipmentForPostOffice(
                    weight, length, width, height,
                    startPostOffice.Type.MaxShipmentWeight,
                    startPostOffice.Type.MaxShipmentLength,
                    startPostOffice.Type.MaxShipmentWidth,
                    startPostOffice.Type.MaxShipmentHeight);

            if (endPostOfficeValidationResult.IsFailure)
            {
                logger.LogError(endPostOfficeValidationResult.Error);
                return Result.Failure<Shipment>(endPostOfficeValidationResult.Error);
            }

            var price = ShipmentPriceCalculator.CalculatePrice(weight, appraisedValue, startPostOffice, endPostOffice);

            var id = Guid.NewGuid();
            var trackingNumber = GenerateTrackingNumber();

            var shipmentResult = Shipment.Create(
                id,
                senderId,
                recipientId,
                startPostOfficeId,
                endPostOfficeId,
                payer,
                trackingNumber,
                price,
                appraisedValue,
                weight,
                length,
                width,
                height);

            if (shipmentResult.IsFailure)
            {
                logger.LogError(shipmentResult.Error);
                return Result.Failure<Shipment>(shipmentResult.Error);
            }

            var shipment = shipmentResult.Value;

            await shipmentsRepository.CreateAsync(shipment);

            logger.LogInformation($"Success create shipment. Id: {id}");
            return Result.Success(shipment);
        }

        public async Task<Result<string>> AcceptShipmentAsync(Guid shipmentId, Guid postOfficeId)
        {
            logger.LogInformation($"Start accept shipment with id {shipmentId} " +
                $"at post office with id: {postOfficeId}");

            var shipment = await shipmentsRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                logger.LogError($"Shipment with id: {shipmentId} wasn't found");
                return Result.Failure<string>($"Shipment with id: {shipmentId} wasn't found");
            }

            var postOfficeResult = await postOfficeService.GetPostOfficeByIdAsync(postOfficeId);

            if (postOfficeResult.IsFailure)
            {
                logger.LogError(postOfficeResult.Error);
                return Result.Failure<string>(postOfficeResult.Error);
            }

            var postOffice = postOfficeResult.Value;

            if (shipment.Status == ShipmentStatus.Created && shipment.StartPostOfficeId != postOfficeId)
            {
                logger.LogError($"Can't accept shipment with id: {shipmentId} by post office with id: {postOfficeId} " +
                    $"because start post office was indicated with id: {shipment.StartPostOfficeId}. " +
                    $"Try to create shipment with start post office id: {postOfficeId}");

                return Result.Failure<string>($"Can't accept shipment with id: {shipmentId} by post office with id: {postOfficeId} " +
                    $"because start post office was indicated with id: {shipment.StartPostOfficeId}. " +
                    $"Please create shipment with current post office: {postOffice}");
            }

            if (shipment.Status == ShipmentStatus.Delivered)
            {
                logger.LogError($"Can't accept shipment that already has status: Delivered");
                return Result.Failure<string>("Can't accept shipment that already has status: Delivered");
            }

            if (postOffice.Shipments.Count == postOffice.MaxShipmentsCount)
            {
                var errorMessage = $"Post office with id: {postOfficeId} is full! Operator can't accept shipments. " +
                    $"Send some shipments and then try to accept again.";

                logger.LogError(errorMessage);
                return Result.Failure<string>(errorMessage);
            }

            var statusDate = DateTime.Now;

            string description = $"Shipment: {shipment.TrackingNumber} arrived at {postOffice} at {statusDate}";

            ShipmentStatus newShipmentStatus = ShipmentStatus.AtPostOffice;

            if (shipment.Status == ShipmentStatus.Redirected && shipment.EndPostOfficeId != postOfficeId)
            {
                newShipmentStatus = shipment.Status;

                var newShipmentResult = await CreateShipmentAsync(
                    shipment.SenderId,
                    shipment.RecipientId,
                    postOfficeId,
                    shipment.EndPostOfficeId,
                    shipment.Payer,
                    shipment.AppraisedValue,
                    shipment.Weight,
                    shipment.Length,
                    shipment.Width,
                    shipment.Height);

                if (newShipmentResult.IsFailure)
                {
                    logger.LogError(newShipmentResult.Error);
                    return Result.Failure<string>(newShipmentResult.Error);
                }

                var newShipment = newShipmentResult.Value;

                if (shipment.IsPaid)
                {
                    await shipmentsRepository.UpdatePaidStatusAsync(newShipment.Id, true);
                }

                var acceptResult = await AcceptShipmentAsync(newShipment.Id, postOfficeId);

                if (acceptResult.IsFailure)
                {
                    logger.LogError(acceptResult.Error);
                    return Result.Failure<string>(acceptResult.Error);
                }

                description = $"Shipment: {shipment.TrackingNumber} arrived at {postOffice} at {statusDate} and redirected. " +
                    $"New tracking number: {newShipment.TrackingNumber}";
            }

            if (shipment.EndPostOfficeId == postOfficeId)
            {
                description = $"Shipment: {shipment.TrackingNumber} was delivered at {postOffice} at {statusDate}";

                newShipmentStatus = ShipmentStatus.Delivered;

                var message = $"Shipment: {shipment.TrackingNumber} was delivered at {postOffice}";

                await notificationService.NotifyShipmentArrivedAsync(shipment.SenderId, message);
                await notificationService.NotifyShipmentArrivedAsync(shipment.RecipientId, message);

                if (shipment.ConfidantId.HasValue)
                {
                    await notificationService.NotifyShipmentArrivedAsync(shipment.ConfidantId.Value, message);
                }
            }

            var shipmentHistoryResult = ShipmentHistory.Create(
                Guid.NewGuid(),
                shipmentId,
                newShipmentStatus,
                postOfficeId,
                statusDate,
                description);

            if (shipmentHistoryResult.IsFailure)
            {
                logger.LogError(shipmentHistoryResult.Error);
                return Result.Failure<string>(shipmentHistoryResult.Error);
            }

            var shipmentHistory = shipmentHistoryResult.Value;

            if (newShipmentStatus != ShipmentStatus.Redirected)
            {
                await shipmentsRepository.UpdateStatusAsync(shipmentId, newShipmentStatus);
            }

            await shipmentsRepository.UpdateCurrentPostOfficeAsync(shipmentId, postOfficeId);
            await shipmentsRepository.AddHistoryAsync(shipmentId, shipmentHistory);

            logger.LogInformation($"Success accept shipment with id {shipmentId} " +
                $"at post office with id: {postOfficeId} ");

            return Result.Success($"Shipment with tracking number: {shipment.TrackingNumber} " +
                $"accepted by {postOffice} successfully");
        }

        public async Task<Result<string>> SendShipmentAsync(Guid shipmentId, Guid operatorPostOfficeId)
        {
            logger.LogInformation($"Start send shipment with id {shipmentId} " +
                $"from post office");

            var shipment = await shipmentsRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                logger.LogError($"Shipment with id: {shipmentId} wasn't found");
                return Result.Failure<string>($"Shipment with id: {shipmentId} wasn't found");
            }

            if (!shipment.CurrentPostOfficeId.HasValue)
            {
                logger.LogError($"Shipment doesn't have current post office");
                return Result.Failure<string>($"Shipment doesn't have current post office");
            }

            if (operatorPostOfficeId != shipment.CurrentPostOfficeId)
            {
                logger.LogError($"Operator with post office id: {operatorPostOfficeId} doesn't work at this post office " +
                    $"and can't receive shipment with id: {shipmentId}.");

                return Result.Failure<string>($"Operator with post office id: {operatorPostOfficeId} doesn't work " +
                    $"at this post office and can't receive shipment with id: {shipmentId}.");
            }

            var postOfficeResult = await postOfficeService.GetPostOfficeByIdAsync(shipment.CurrentPostOfficeId.Value);

            if (postOfficeResult.IsFailure)
            {
                logger.LogError(postOfficeResult.Error);
                return Result.Failure<string>(postOfficeResult.Error);
            }

            var postOffice = postOfficeResult.Value;

            if (shipment.Status == ShipmentStatus.Delivered)
            {
                logger.LogError($"Can't send shipment that already has status: {ShipmentStatus.Delivered}");
                return Result.Failure<string>($"Can't send shipment that already has status: {ShipmentStatus.Delivered}");
            }

            if (shipment.Status == ShipmentStatus.Redirected)
            {
                logger.LogError($"Can't send shipment that has status: {ShipmentStatus.Redirected}");
                return Result.Failure<string>($"Can't send shipment that has status: {ShipmentStatus.Redirected}");
            }

            if (shipment.Status == ShipmentStatus.Created)
            {
                logger.LogError($"Shipment has status: {ShipmentStatus.Created} and can't be send." +
                    $"Shipment should has status: {ShipmentStatus.AtPostOffice}");

                return Result.Failure<string>($"Shipment has status: {ShipmentStatus.Created} and can't be send." +
                    $"Shipment should has status: {ShipmentStatus.AtPostOffice}");
            }

            var statusDate = DateTime.Now;

            ShipmentStatus newShipmentStatus = ShipmentStatus.InTransit;

            string description = $"Shipment: {shipment.TrackingNumber} left from {postOffice} at {statusDate}";

            var shipmentHistoryResult = ShipmentHistory.Create(
                Guid.NewGuid(),
                shipmentId,
                newShipmentStatus,
                shipment.CurrentPostOfficeId.Value,
                statusDate,
                description);

            if (shipmentHistoryResult.IsFailure)
            {
                logger.LogError(shipmentHistoryResult.Error);
                return Result.Failure<string>(shipmentHistoryResult.Error);
            }

            var shipmentHistory = shipmentHistoryResult.Value;

            await shipmentsRepository.UpdateCurrentPostOfficeAsync(shipment.Id, null);
            await shipmentsRepository.UpdateStatusAsync(shipmentId, newShipmentStatus);
            await shipmentsRepository.AddHistoryAsync(shipmentId, shipmentHistory);

            logger.LogInformation($"Shipment with id: {shipment.Id} left from {postOffice} at {statusDate} successfully. " +
                $"Operator post office id: {operatorPostOfficeId}");

            return Result.Success($"Shipment with tracking number: {shipment.TrackingNumber} " +
                $"left from {postOffice} at {statusDate} successfully");
        }

        public async Task<IEnumerable<Shipment>> GetAllShipmentsAsync()
        {
            logger.LogInformation($"Start get shipments");
            var shipments = await shipmentsRepository.GetAllAsync();

            logger.LogInformation($"Success get shipments");
            return shipments;
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByPostOfficeIdAsync(Guid postofficeId)
        {
            logger.LogInformation($"Start get post office shipments. " +
                $"Post office id: {postofficeId}");
            var shipments = await shipmentsRepository.GetByPostOfficeIdAsync(postofficeId);

            logger.LogInformation($"Success get shipments by post office id: {postofficeId}");
            return shipments;
        }

        public async Task<Result<Shipment>> GetShipmentByTrackingNumberAsync(string trackingNumber)
        {
            logger.LogInformation($"Start get shipment with id: {trackingNumber}");
            var shipment = await shipmentsRepository.GetByTrackingNumberAsync(trackingNumber);

            if (shipment == null)
            {
                logger.LogError($"Shipment with tracking number: {trackingNumber} wasn't found");
                return Result.Failure<Shipment>($"Shipment with tracking number: {trackingNumber} wasn't found");
            }

            logger.LogInformation($"Success get shipment with tracking number: {trackingNumber}");
            return Result.Success(shipment);
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByUserIdAsync(Guid userId)
        {
            logger.LogInformation($"Start get shipments by user id: {userId}");

            var shipments = await shipmentsRepository.GetByUserIdAsync(userId);

            logger.LogInformation($"Success get shipments");
            return shipments;
        }

        public async Task<Result<Shipment>> GetShipmentByIdAsync(Guid shipmentId)
        {
            logger.LogInformation($"Start get shipment with id: {shipmentId}");
            var shipment = await shipmentsRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                logger.LogError($"Shipment with id: {shipmentId} wasn't found");
                return Result.Failure<Shipment>($"Shipment with id: {shipmentId} wasn't found");
            }

            logger.LogInformation($"Success get shipment with tracking number: {shipmentId}");
            return Result.Success(shipment);
        }

        public async Task<Result<string>> UpdateShipmentPaidStatusAsync(Guid shipmentId)
        {
            logger.LogInformation($"Start update paid status shipment with id: {shipmentId}");
            await shipmentsRepository.UpdatePaidStatusAsync(shipmentId, true);

            logger.LogInformation($"Success update paid status shipment");
            return Result.Success($"Shipment with id: {shipmentId} paid status updated successfully");
        }

        public async Task<Result<string>> RedirectShipmentAsync(Guid userId, Guid shipmentId, Guid newEndPostOfficeId)
        {
            logger.LogInformation($"Start redirect shipment with id: {shipmentId} " +
                $"by user with id {userId} to new end post office with id: {newEndPostOfficeId}");

            var shipment = await shipmentsRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                logger.LogError($"Shipment with id {shipmentId} wasn't found");
                return Result.Failure<string>($"Shipment with id {shipmentId} wasn't found");
            }

            if (shipment.SenderId != userId && shipment.RecipientId != userId)
            {
                logger.LogError($"User with id: {userId} doesn't have permission to redirect this shipment");
                return Result.Failure<string>($"User with id: {userId} doesn't have permission to redirect this shipment");
            }

            if (shipment.Status == ShipmentStatus.Created)
            {
                logger.LogError($"Shipment with id: {shipmentId} has status CREATED. " +
                    $"Shipment can be redirected before accept in post office. " +
                    $"You can try to create shipment with another end post office");

                return Result.Failure<string>($"Shipment with id: {shipmentId} has status CREATED. " +
                    $"Shipment can be redirected before accept in post office.");
            }

            if (shipment.Status == ShipmentStatus.Redirected)
            {
                logger.LogError($"Shipment with id: {shipmentId} has status REDIRECTED. " +
                    $"Shipment can be redirected again.");

                return Result.Failure<string>($"Shipment with id: {shipmentId} has status REDIRECTED. " +
                    $"Shipment can be redirected againt.");
            }

            if (shipment.EndPostOfficeId == newEndPostOfficeId)
            {
                logger.LogError($"End post office and new end post office can't be equel");
                return Result.Failure<string>($"End post office and new end post office can't be equel");
            }

            var newEndPostOfficeResult = await postOfficeService.GetPostOfficeByIdAsync(newEndPostOfficeId);

            if (newEndPostOfficeResult.IsFailure)
            {
                logger.LogError(newEndPostOfficeResult.Error);
                return Result.Failure<string>(newEndPostOfficeResult.Error);
            }

            var newEndPostOffice = newEndPostOfficeResult.Value;

            var status = ShipmentStatus.Redirected;

            var resultMessage = $"Shipment: {shipment.TrackingNumber} update end post office successfully. " +
                $"New post office: {newEndPostOffice}";

            if (shipment.CurrentPostOfficeId.HasValue && shipment.CurrentPostOfficeId.Value == newEndPostOfficeId)
            {
                var statusDate = DateTime.Now;

                string description = $"Shipment: {shipment.TrackingNumber} delivered successfully;";

                status = ShipmentStatus.Delivered;

                var shipmentHistoryResult = ShipmentHistory.Create(
                Guid.NewGuid(),
                shipmentId,
                status,
                shipment.CurrentPostOfficeId.Value,
                statusDate,
                description);

                if (shipmentHistoryResult.IsFailure)
                {
                    logger.LogError(shipmentHistoryResult.Error);
                    return Result.Failure<string>(shipmentHistoryResult.Error);
                }

                var shipmentHistory = shipmentHistoryResult.Value;

                await shipmentsRepository.AddHistoryAsync(shipmentId, shipmentHistory);

                string message = $"Shipment with id: {shipmentId} arrived to postOffice: {newEndPostOffice}";

                await notificationService.NotifyShipmentArrivedAsync(shipment.SenderId, message);
                await notificationService.NotifyShipmentArrivedAsync(shipment.RecipientId, message);

                if (shipment.ConfidantId.HasValue)
                {
                    await notificationService.NotifyShipmentArrivedAsync(shipment.ConfidantId.Value, message);
                }

                resultMessage = $"Shipment: {shipment.TrackingNumber} delivered successfully";
            }

            await shipmentsRepository.UpdateStatusAsync(shipmentId, status);
            await shipmentsRepository.UpdateEndPostOfficeAsync(shipmentId, newEndPostOfficeId);

            logger.LogInformation(resultMessage);

            return Result.Success(resultMessage);
        }

        public async Task<Result<string>> AddConfidantShipmentAsync(Guid userId, Guid shipmentId, string confidantPhone)
        {
            logger.LogInformation($"Start add confidan to shipment with id: {shipmentId}");
            var shipment = await shipmentsRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                logger.LogError($"Shipment with id {shipmentId} wasn't found");
                return Result.Failure<string>($"Shipment with id {shipmentId} wasn't found");
            }

            if (shipment.SenderId != userId && shipment.RecipientId != userId)
            {
                logger.LogError($"User with id: {userId} doesn't have permission to add confidant");
                return Result.Failure<string>($"User with id: {userId} doesn't have permission to add confidant");
            }

            var confidantResult = await userService.GetUserByPhoneAsync(confidantPhone);

            if (confidantResult.IsFailure)
            {
                logger.LogError(confidantResult.Error);
                return Result.Failure<string>(confidantResult.Error);
            }

            var confidant = confidantResult.Value;

            if (shipment.RecipientId == confidant.Id)
            {
                logger.LogError($"Recipient and Confidant can't be equel person");
                return Result.Failure<string>("Recipient and Confidant can't be equel person");
            }

            await shipmentsRepository.UpdateConfidantAsync(shipmentId, confidant.Id);

            logger.LogInformation($"Shipment {shipment.TrackingNumber} update confidant successfully");
            return Result.Success($"Shipment {shipment.TrackingNumber} update confidant successfully");
        }

        public async Task<Result<string>> RemoveConfidantShipmentAsync(Guid userId, Guid shipmentId)
        {
            logger.LogInformation($"Start remove confidant to shipment with id: {shipmentId}");
            var shipment = await shipmentsRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                logger.LogError($"Shipment with id {shipmentId} wasn't found");
                return Result.Failure<string>($"Shipment with id {shipmentId} wasn't found");
            }

            if (shipment.SenderId != userId && shipment.ConfidantId != userId)
            {
                logger.LogError($"User with id: {userId} doesn't have permission to remove confidant");
                return Result.Failure<string>($"User with id: {userId} doesn't have permission to remove confidant");
            }

            if (!shipment.ConfidantId.HasValue)
            {
                logger.LogError($"Shipment doesn't have confidant");
                return Result.Failure<string>($"Shipment doesn't have confidant");
            }

            await shipmentsRepository.UpdateConfidantAsync(shipment.Id, null);

            logger.LogInformation($"Confidant remove successfully");
            return Result.Success($"Confidant remove successfully");
        }

        public async Task<Result<string>> ReceiveShipmentAsync(Guid recipientId, Guid shipmentId, Guid operatorId)
        {
            var operatorResult = await operatorService.GetByIdAsync(operatorId);

            if (operatorResult.IsFailure)
            {
                logger.LogError(operatorResult.Error);
                return Result.Failure<string>(operatorResult.Error);
            }

            var operatorPostOffice = operatorResult.Value;

            logger.LogInformation($"Start receive shipment with id: {shipmentId} " +
                $"by user with id: {recipientId}. Operator id: {operatorPostOffice.Id}");

            var userResult = await userService.GetUserByIdAsync(recipientId);

            if (userResult.IsFailure)
            {
                logger.LogError(userResult.Error);
                return Result.Failure<string>(userResult.Error);
            }

            var user = userResult.Value;

            var shipment = await shipmentsRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                logger.LogError($"Shipment with id {shipmentId} wasn't found");
                return Result.Failure<string>($"Shipment with id: {shipmentId} wasn't found");
            }

            if (!shipment.CurrentPostOfficeId.HasValue)
            {
                logger.LogError($"Current post office wasn't found");
                return Result.Failure<string>("Current post office wasn't found");
            }

            if (operatorPostOffice.PostOfficeId != shipment.CurrentPostOfficeId)
            {
                logger.LogError($"Operator with id: {operatorPostOffice.Id} doesn't work at this post office " +
                    $"and can't receive shipment with id: {shipmentId}.");

                return Result.Failure<string>($"Operator with id: {operatorPostOffice.Id} doesn't work " +
                    $"at this post office and can't receive shipment with id: {shipmentId}.");
            }

            if (!shipment.IsPaid)
            {
                logger.LogError($"Shipment wasn't paid");
                return Result.Failure<string>($"Shipment wasn't paid");
            }

            var shipmentHistoryResult = ShipmentHistory.Create(
                Guid.NewGuid(),
                shipment.Id,
                ShipmentStatus.Received,
                shipment.CurrentPostOfficeId.Value,
                DateTime.Now,
                $"Shipment with tracking number: {shipment.TrackingNumber} was received");

            if (shipmentHistoryResult.IsFailure)
            {
                logger.LogError(shipmentHistoryResult.Error);
                return Result.Failure<string>(shipmentHistoryResult.Error);
            }

            var shipmentHistory = shipmentHistoryResult.Value;

            if (!shipment.ConfidantId.HasValue && shipment.RecipientId != user.Id)
            {
                logger.LogError($"User with Id: {recipientId} isn't recipient for this shipment");
                return Result.Failure<string>($"User with Id: {recipientId} isn't recipient for this shipment");
            }

            await shipmentsRepository.UpdateCurrentPostOfficeAsync(shipment.Id, null);
            await shipmentsRepository.UpdateStatusAsync(shipment.Id, ShipmentStatus.Received);
            await shipmentsRepository.AddHistoryAsync(shipment.Id, shipmentHistory);
            await shipmentsRepository.AddOperatorWhoIssuedId(shipment.Id, operatorPostOffice.Id);

            logger.LogInformation($"Shipment with id: {shipment.Id} was receive successfully " +
                $"by user with id: {recipientId}. Operator id: {operatorPostOffice.Id}");
            return Result.Success($"Shipment with tracking number: {shipment.TrackingNumber} was receive successfully");
        }

        public async Task<Result<string>> SoftDeleteShipmentAsync(Guid shipmentId, Guid userId)
        {
            logger.LogInformation($"Start soft delete shipment with id: {shipmentId} " +
                $"by user with id: {userId}");

            var shipment = await shipmentsRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                logger.LogError($"Shipment with id {shipmentId} wasn't found");
                return Result.Failure<string>($"Shipment with id: {shipmentId} wasn't found");
            }

            if (userId != shipment.SenderId &&
                userId != shipment.RecipientId &&
                userId != shipment.ConfidantId)
            {
                logger.LogError($"User with id: {userId} can't delete this shipment");
                return Result.Failure<string>($"User with id: {userId} can't delete this shipment");
            }

            await shipmentsRepository.SoftDeleteAsync(shipmentId, userId);

            logger.LogInformation($"Shipment: {shipment.TrackingNumber} deleted for user: {userId} successfully");
            return Result.Success($"Shipment: {shipment.TrackingNumber} deleted for user: {userId} successfully");
        }

        private string GenerateTrackingNumber()
        {
            Random random = new Random();
            char[] digits = new char[Shipment.TRACKING_NUMBER_LENGTH];

            for (int i = 0; i < Shipment.TRACKING_NUMBER_LENGTH; i++)
            {
                digits[i] = (char)('0' + random.Next(0, 10));
            }

            return new string(digits);
        }

        private Result ValidationShipmentForPostOffice(
            float shipmentWeight,
            float shipmentLength,
            float shipmentWidth,
            float shipmentHeight,
            float postOfficeMaxWeight,
            float postOfficeMaxLength,
            float postOfficeMaxWidth,
            float postOfficeMaxHeight)
        {
            if (shipmentWeight > postOfficeMaxWeight)
            {
                return Result.Failure<string>($"Weight in this post office must be equel or less than {postOfficeMaxWeight}");
            }
            else if (shipmentLength > postOfficeMaxLength)
            {
                return Result.Failure<string>($"Length in this post office must be equel or less than {postOfficeMaxLength}");
            }
            else if (shipmentWidth > postOfficeMaxWidth)
            {
                return Result.Failure<string>($"Width in this post office must be equel or less than {postOfficeMaxWidth}");
            }
            else if (shipmentHeight > postOfficeMaxHeight)
            {
                return Result.Failure<string>($"Height in this post office must be equel or less than {postOfficeMaxHeight}");
            }

            return Result.Success();
        }
    }
}
