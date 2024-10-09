using Poshta.Core.Events;

namespace Poshta.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmail(EmailNotificationEventArgs e);
    }
}
