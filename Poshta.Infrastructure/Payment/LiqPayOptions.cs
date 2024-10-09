namespace Poshta.Infrastructure.Payment
{
    public class LiqPayOptions
    {
        public string PublicKey { get; set; } = string.Empty;

        public string PrivateKey { get; set; } = string.Empty;
        
        public string CallbackUrl {  get; set; } = string.Empty;
    }
}
