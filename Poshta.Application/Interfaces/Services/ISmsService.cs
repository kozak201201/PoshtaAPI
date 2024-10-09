using Poshta.Core.Events;

namespace Poshta.Application.Interfaces.Services
{
    public interface ISmsService
    {
        Task SendSms(SmsNotificationEventArgs e);
    }
}
