using Twilio.Types;
using Twilio;
using Microsoft.Extensions.Options;
using Twilio.Rest.Api.V2010.Account;
using Poshta.Application.Interfaces.Services;
using Poshta.Core.Events;

namespace Poshta.Infrastructure.Sms
{
    public class TwilioSmsService(IOptionsMonitor<TwilioOptions> options) : ISmsService
    {
        private readonly IOptionsMonitor<TwilioOptions> options = options;

        public async Task SendSms(SmsNotificationEventArgs e)
        {
            var twilioOptions = options.CurrentValue;

            TwilioClient.Init(twilioOptions.AccountSid, twilioOptions.AuthToken);

            var messageOptions = new CreateMessageOptions(new PhoneNumber(e.Phone))
            {
                From = new PhoneNumber(twilioOptions.PhoneNumber),
                Body = e.Message
            };

            await MessageResource.CreateAsync(messageOptions);
        }
    }
}
