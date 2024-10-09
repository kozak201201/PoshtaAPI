using Poshta.Application.Dtos;

namespace Poshta.Application.Interfaces.Services
{
    public interface IPaymentGateway
    {
        public string GeneratePaymentRequest(PaymentDetails paymentDetails);

        PaymentResult ProcessPaymentResult(Dictionary<string, string> parameters);
    }
}
