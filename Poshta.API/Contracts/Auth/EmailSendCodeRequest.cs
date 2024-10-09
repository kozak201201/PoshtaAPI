using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Auth
{
    public record EmailSendCodeRequest([Required][EmailAddress] string Email);
}
