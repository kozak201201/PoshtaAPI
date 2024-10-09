using Poshta.Core.Events;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using Poshta.Application.Interfaces.Services;

namespace Poshta.Infrastructure.Email
{
    public class SmtpEmailService(IOptionsMonitor<SmtpOptions> options) : IEmailService
    {
        private readonly IOptionsMonitor<SmtpOptions> options = options;

        public async Task SendEmail(EmailNotificationEventArgs e)
        {
            var smtpOptions = options.CurrentValue;

            using (var client = new SmtpClient(smtpOptions.Host, smtpOptions.Port))
            {
                client.Credentials = new NetworkCredential(smtpOptions.User, smtpOptions.Password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpOptions.User),
                    Subject = e.Subject,
                    Body = e.Body,
                    IsBodyHtml = false
                };

                if (string.IsNullOrEmpty(e.Email))
                {
                    throw new Exception("Email can't be null or empty");
                }

                mailMessage.To.Add(e.Email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
