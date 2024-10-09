namespace Poshta.Application.Dtos
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public Guid ShipmentId { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string ResultUrl { get; set; } = string.Empty;
    }
}
