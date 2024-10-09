using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Auth
{
    public record LoginUserRequest(
        [Required, Phone] string Phone,
        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        string Password);
}
