using Microsoft.Extensions.DependencyInjection;
using Poshta.Application.Auth;
using Poshta.Application.Interfaces.Services;
using Poshta.Infrastructure.Cache;
using Poshta.Infrastructure.Email;
using Poshta.Infrastructure.Jwt;
using Poshta.Infrastructure.Sms;
using Poshta.Infrastructure.Payment;

namespace Poshta.Infrastructure
{
    public static class InfrustructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IJwtProvider, JwtProvider>();
            services.AddTransient<ISmsService, TwilioSmsService>();
            services.AddTransient<IEmailService, SmtpEmailService>();
            services.AddTransient<IConfirmationCodeService, ConfirmationCodeService>();
            services.AddTransient<IPaymentGateway, LiqPayGateway>();

            return services;
        }
    }
}
