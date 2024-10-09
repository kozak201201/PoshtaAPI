using Microsoft.Extensions.DependencyInjection;
using Poshta.Application.Interfaces.Services;
using Poshta.Application.Services;
using Poshta.Core.Interfaces.Services;

namespace Poshta.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IOperatorService, OperatorService>();
            services.AddTransient<IShipmentService, ShipmentService>();
            services.AddTransient<IPostOfficeTypeService, PostOfficeTypeService>();
            services.AddTransient<IPostOfficeService, PostOfficeService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<INotificationService, NotificationService>();

            return services;
        }
    }
}
