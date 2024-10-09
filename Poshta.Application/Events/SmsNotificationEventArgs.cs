namespace Poshta.Core.Events
{
    public class SmsNotificationEventArgs : EventArgs
    {
        public string Phone { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
