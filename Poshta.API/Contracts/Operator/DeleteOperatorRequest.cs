using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Operator
{
    public record DeleteOperatorRequest(
        [Required] Guid OperatorId);
}
