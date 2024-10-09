using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Poshta.API.Exceptions;
using Poshta.API.Extensions;
using Poshta.Application;
using Poshta.Application.Mappings;
using Poshta.DataAccess.SqlServer;
using Poshta.DataAccess.SqlServer.Mappings;
using Poshta.Infrastructure;
using Poshta.Infrastructure.Email;
using Poshta.Infrastructure.Jwt;
using Poshta.Infrastructure.Payment;
using Poshta.Infrastructure.Sms;

namespace Poshta.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Env.Load("../.env");

            var services = builder.Services;
            var configuration = builder.Configuration;

            builder.Configuration
                .AddEnvironmentVariables();

            // Add services to the container.

            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            services.Configure<TwilioOptions>(configuration.GetSection(nameof(TwilioOptions)));
            services.Configure<SmtpOptions>(configuration.GetSection(nameof(SmtpOptions)));
            services.Configure<LiqPayOptions>(configuration.GetSection(nameof(LiqPayOptions)));
            services.Configure<ServerOptions>(configuration.GetSection(nameof(ServerOptions)));

            services.AddMemoryCache();

            services.AddHttpContextAccessor();

            services.AddApiAuthentication(configuration);

            services
                .AddDataAccess(configuration)
                .AddApplication()
                .AddInfrastructure();

            services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddAutoMapper(typeof(DataBaseMappings), typeof(DtosMappings));

            services.AddTransient<SeedData>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseExceptionHandler();

            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.Database.Migrate();

                var seedData = scope.ServiceProvider.GetRequiredService<SeedData>();
                await seedData.SeedDataAsync();
            }

            app.Run();
        }
    }
}
