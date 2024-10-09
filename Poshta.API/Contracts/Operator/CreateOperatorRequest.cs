using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Operator
{
    public record CreateOperatorRequest(
        [Required] Guid UserId,
        [Required] Guid PostOfficeId);
}
