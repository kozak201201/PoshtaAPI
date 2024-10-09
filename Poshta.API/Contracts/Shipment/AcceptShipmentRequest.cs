using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Shipment
{
    public record AcceptShipmentRequest(
        [Required] Guid PostOfficeId);
}
