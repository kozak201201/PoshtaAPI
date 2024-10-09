using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Poshta.Application.Dtos;
using Poshta.Application.Interfaces.Services;
using Poshta.Core.Interfaces.Services;
using PayerType = Poshta.Core.Models.PayerType;

namespace Poshta.Application.Services
{
    public class PaymentService(
        IShipmentService shipmentService,
        IPaymentGateway paymentGateway,
        ILogger<PaymentService> logger,
        IOptions<ServerOptions> options) : IPaymentService
    {
        private readonly IShipmentService shipmentService = shipmentService;
        private readonly IPaymentGateway paymentGateway = paymentGateway;
        private readonly ILogger<PaymentService> logger = logger;
        private readonly ServerOptions serverOptions = options.Value;

        public async Task<Result<string>> CreatePaymentRequestUrlAsync(Guid shipmentId, Guid userId)
        {
            logger.LogInformation($"Start create payment request url for shipment with id: {shipmentId}");
            var shipmentResult = await shipmentService.GetShipmentByIdAsync(shipmentId);

            if (shipmentResult.IsFailure)
                return Result.Failure<string>($"Shipment with id: {shipmentId} wasn't found");

            var shipment = shipmentResult.Value;

            if (shipment.IsPaid)
            {
                logger.LogError($"shipment with id: {shipmentId} has already paid");
                return Result.Failure<string>($"shipment with id: {shipmentId} has already paid");
            }

            if (shipment.Payer == PayerType.Sender && shipment.SenderId != userId)
            {
                logger.LogError($"User with id: {userId} can't pay for this shipment with id: {shipmentId}");
                return Result.Failure<string>($"User with id: {userId} can't pay for this shipment with id: {shipmentId}");
            }
            else if (shipment.Payer == PayerType.Recipient &&
                shipment.RecipientId != userId &&
                shipment.ConfidantId != userId)
            {
                logger.LogError($"User with id: {userId} can't pay for this shipment with id: {shipmentId}");
                return Result.Failure<string>($"User with id: {userId} can't pay for this shipment with id: {shipmentId}");
            }

            var paymentDetails = new PaymentDetails()
            {
                Amount = shipment.Price,
                Description = shipmentId.ToString(),
                OrderId = Guid.NewGuid().ToString(),
                ResultUrl = $"{serverOptions.ServerUrl}/api/payment/process-payment-result"
            };

            var paymentUrl = paymentGateway.GeneratePaymentRequest(paymentDetails);

            logger.LogInformation($"Success create payment request url for shipment with id: {shipmentId}");
            return Result.Success(paymentUrl);
        }

        public async Task<Result<string>> ProcessPaymentResult(Dictionary<string, string> parameters)
        {
            logger.LogInformation($"Start process payment result");

            if (!parameters.ContainsKey("data") || !parameters.ContainsKey("signature"))
            {
                logger.LogError("Missing 'data' or 'signature' in form.");
                return Result.Failure<string>("Missing required payment result parameters.");
            }

            var result = paymentGateway.ProcessPaymentResult(parameters);

            if (!result.IsSuccess)
            {
                logger.LogError("payment result failure");
                return Result.Failure<string>(result.ErrorMessage);
            }

            var updateShipmentPaidResult = await shipmentService.UpdateShipmentPaidStatusAsync(result.ShipmentId);

            if (updateShipmentPaidResult.IsFailure)
            {
                logger.LogError($"update shipment paid result was failure: {result.ErrorMessage}");
                return Result.Failure<string>(result.ErrorMessage);
            }

            logger.LogInformation($"Shipment with id: {result.ShipmentId} paid successfully");
            return Result.Success($"Shipment with id: {result.ShipmentId} paid successfully");
        }
    }
}
