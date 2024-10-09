using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Operator
{
    public record UpdateOperatorPostOfficeRequest(
        [Required] Guid NewPostOfficeId);
}
