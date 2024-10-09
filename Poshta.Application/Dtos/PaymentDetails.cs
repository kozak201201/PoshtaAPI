namespace Poshta.Application.Dtos
{
    public class PaymentDetails
    {
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string ResultUrl { get; set; } = string.Empty;
    }
}
