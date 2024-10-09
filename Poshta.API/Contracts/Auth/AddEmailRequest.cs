using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Auth
{
    public record AddEmailRequest(
        [Required][EmailAddress] string Email,
        [Required] string ConfirmationCode);
}
