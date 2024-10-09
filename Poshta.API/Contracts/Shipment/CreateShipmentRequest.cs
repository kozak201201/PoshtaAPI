using Poshta.Core.Models;
using System.ComponentModel.DataAnnotations;
using ShipmentModel = Poshta.Core.Models.Shipment;

namespace Poshta.API.Contracts.Shipment
{
    public record CreateShipmentRequest(
        [Required] Guid SenderId,
        [Required] Guid RecipientId,
        [Required] Guid StartPostOfficeId,
        [Required] Guid EndPostOfficeId,

        [Required][EnumDataType(typeof(PayerType))] PayerType Payer,

        [Range(ShipmentModel.MIN_APPRAISED_VALUE, ShipmentModel.MAX_APPRAISED_VALUE,
        ErrorMessage = "AppraisedValue must be between {1} and {2}")]
        double? AppraisedValue = ShipmentModel.MIN_APPRAISED_VALUE,

        [Range(ShipmentModel.MIN_WEIGHT, float.MaxValue, 
        ErrorMessage = "Weight must be greater than {1}")]
        float? Weight = ShipmentModel.DEFAULT_WEIGHT,

        [Range(ShipmentModel.MIN_LENGTH, float.MaxValue, 
        ErrorMessage = "Length must be greater than {1}")]
        float? Length = ShipmentModel.MIN_LENGTH,

        [Range(ShipmentModel.MIN_WIDTH, float.MaxValue,
        ErrorMessage = "Width must be greater than {1}")]
        float? Width = ShipmentModel.MIN_WIDTH,

        [Range(ShipmentModel.MIN_HEIGHT, float.MaxValue,
        ErrorMessage = "Height must be greater than {1}")]
        float? Height = ShipmentModel.MIN_HEIGHT);
}
