using CSharpFunctionalExtensions;
using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Services
{
    public interface IShipmentService
    {
        Task<Result<Shipment>> CreateShipmentAsync(
            Guid senderId,
            Guid recipientId,
            Guid startPostOfficeId,
            Guid endPostOfficeId,
            PayerType payerType,
            double appraisedValue,
            float weight,
            float length,
            float width,
            float height);

        Task<IEnumerable<Shipment>> GetShipmentsByUserIdAsync(Guid userId);

        Task<IEnumerable<Shipment>> GetAllShipmentsAsync();

        Task<IEnumerable<Shipment>> GetShipmentsByPostOfficeIdAsync(Guid postofficeId);

        Task<Result<Shipment>> GetShipmentByIdAsync(Guid shipmentId);

        Task<Result<Shipment>> GetShipmentByTrackingNumberAsync(string trackingNumber);

        Task<Result<string>> AcceptShipmentAsync(Guid shipmentId, Guid postOfficeId);

        Task<Result<string>> SendShipmentAsync(Guid shipmentId, Guid operatorUserId);

        Task<Result<string>> UpdateShipmentPaidStatusAsync(Guid shipmentId);

        Task<Result<string>> RedirectShipmentAsync(Guid userId, Guid shipmentId, Guid newEndPostOfficeId);

        Task<Result<string>> ReceiveShipmentAsync(Guid userId, Guid shipmentId, Guid operatorUserId);

        Task<Result<string>> AddConfidantShipmentAsync(Guid userId, Guid shipmentId, string confidantPhone);

        Task<Result<string>> RemoveConfidantShipmentAsync(Guid userId, Guid shipmentId);

        Task<Result<string>> SoftDeleteShipmentAsync(Guid shipmentId, Guid userId);
    }
}