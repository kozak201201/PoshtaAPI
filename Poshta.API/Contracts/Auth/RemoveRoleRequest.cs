using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Auth
{
    public record RemoveRoleRequest(
        [Required] string Role);
}
