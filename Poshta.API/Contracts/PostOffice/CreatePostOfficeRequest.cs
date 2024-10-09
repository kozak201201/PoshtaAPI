using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.PostOffice
{
    public record CreatePostOfficeRequest(
        [Required] int Number,
        [Required] string City,
        [Required] string Address,
        [Required] int MaxShipmentsCount,
        [Required] double Latitude,
        [Required] double Longitude,
        [Required] Guid PostOfficeTypeId);
}
