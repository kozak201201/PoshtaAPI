using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Poshta.Application.Interfaces.Services;
using Poshta.Application.Dtos;

namespace Poshta.Infrastructure.Payment
{
    public class LiqPayGateway : IPaymentGateway
    {
        private readonly LiqPayOptions payOptions;

        public LiqPayGateway(IOptions<LiqPayOptions> options)
        {
            payOptions = options.Value;
        }

        public string GeneratePaymentRequest(PaymentDetails paymentDetails)
        {
            var paymentData = new
            {
                version = "3",
                public_key = payOptions.PublicKey,
                action = "pay",
                amount = paymentDetails.Amount,
                currency = "UAH",
                description = paymentDetails.Description,
                order_id = paymentDetails.OrderId,
            };

            string json = JsonConvert.SerializeObject(paymentData);
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            string signature = GenerateSignature(base64);

            return $"https://www.liqpay.ua/api/3/checkout?data={base64}&signature={signature}";
        }

        public PaymentResult ProcessPaymentResult(Dictionary<string, string> parameters)
        {
            var data = parameters["data"];
            var signature = parameters["signature"];

            var expectedSignature = GenerateSignature(data);

            if (signature != expectedSignature)
            {
                return new PaymentResult { IsSuccess = false, ErrorMessage = "Invalid signature" };
            }

            var decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            var paymentInfo = JsonConvert.DeserializeObject<LiqPayPaymentResponse>(decodedData);

            if (paymentInfo == null)
            {
                return new PaymentResult { IsSuccess = false, ErrorMessage = "Invalid payment information" };
            }

            if (paymentInfo.status != "success")
            {
                return new PaymentResult { IsSuccess = false, ErrorMessage = "Payment status isn't success" };
            }

            return new PaymentResult
            {
                IsSuccess = true,
                ShipmentId = paymentInfo.description,
            };
        }

        private string GenerateSignature(string data)
        {
            var signature = payOptions.PrivateKey + data + payOptions.PrivateKey;
            var hash = SHA1.HashData(Encoding.UTF8.GetBytes(signature));

            return Convert.ToBase64String(hash);
        }

        private class LiqPayPaymentResponse
        {
            public string status { get; set; } = string.Empty;
            public Guid description { get; set; }
        }
    }
}
