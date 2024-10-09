using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Shipment
{
    public record ReceiveShipmentRequest(
        [Required] Guid RecipientId,
        [Required] Guid OperatorId);
}
