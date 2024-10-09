using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Auth
{
    public record AddRoleRequest(
        [Required] string Role);
}
