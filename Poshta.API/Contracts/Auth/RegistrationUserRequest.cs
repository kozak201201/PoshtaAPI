using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Auth
{
    public record RegistrationUserRequest(
        [Required] string LastName,
        [Required] string FirstName,
        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")] string Password,
        [Required, Phone] string Phone,
        [Required] string ConfirmationCode,
        string? MiddleName = null);
}
