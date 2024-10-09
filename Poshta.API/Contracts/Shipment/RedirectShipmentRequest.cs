using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Shipment
{
    public record RedirectShipmentRequest(
        [Required] Guid NewEndPostOfficeId);
}
