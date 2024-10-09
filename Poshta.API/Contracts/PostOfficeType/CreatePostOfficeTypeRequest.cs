using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.PostOfficeType
{
    public record CreatePostOfficeTypeRequest(
        [Required] string Name,
        [Required] float MaxShipmentWeight,
        [Required] float MaxShipmentLength,
        [Required] float MaxShipmentWidtht,
        [Required] float MaxShipmentHeight);
}
