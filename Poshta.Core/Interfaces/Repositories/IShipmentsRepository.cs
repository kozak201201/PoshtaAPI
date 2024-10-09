using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Repositories
{
    public interface IShipmentsRepository
    {
        Task CreateAsync(Shipment shipment);

        Task<IEnumerable<Shipment>> GetAllAsync();

        Task<IEnumerable<Shipment>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<Shipment>> GetByPostOfficeIdAsync(Guid postOfficeId);

        Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber);

        Task<Shipment?> GetByIdAsync(Guid shipmentId);

        Task UpdatePaidStatusAsync(Guid shipmentId, bool newPaidStatus);

        Task UpdateStatusAsync(Guid shipmentId, ShipmentStatus status);
        
        Task UpdateConfidantAsync(Guid shipmentId, Guid? confidantId);
        
        Task UpdateCurrentPostOfficeAsync(Guid shipmentId, Guid? newCurrentPostOffice);
        
        Task UpdateEndPostOfficeAsync(Guid shipmentId, Guid newEndPostOfficeId);
        
        Task AddHistoryAsync(Guid shipmentId, ShipmentHistory shipmentHistory);

        Task AddOperatorWhoIssuedId(Guid shipmentId, Guid operatorId);

        Task SoftDeleteAsync(Guid shipmentId, Guid userId);
    }
}