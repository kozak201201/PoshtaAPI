using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poshta.Core.Interfaces.Repositories;
using Poshta.DataAccess.SqlServer.Entities;
using Poshta.DataAccess.SqlServer.Repositories;

namespace Poshta.DataAccess.SqlServer
{
    public static class DataAccessExtentions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityCore<UserEntity>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = false;
            }).AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            var connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IShipmentsRepository, ShipmentsRepository>();
            services.AddScoped<IPostOfficeTypesRepository, PostOfficeTypesRepository>();
            services.AddScoped<IPostOfficesRepository, PostOfficesRepository>();
            services.AddScoped<IOperatorsRepository, OperatorsRepository>();

            return services;
        }
    }
}
