using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Operator
{
    public record AddRatingRequest(
        [Required] Guid ShipmentId,
        [Required] int Rating,
        string Review);
}
