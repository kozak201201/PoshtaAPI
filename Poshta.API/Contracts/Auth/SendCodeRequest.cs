using System.ComponentModel.DataAnnotations;

namespace Poshta.API.Contracts.Auth
{
    public record SmsSendCodeRequest([Required][Phone] string Phone);
}
