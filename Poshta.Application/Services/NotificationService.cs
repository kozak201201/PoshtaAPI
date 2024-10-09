using Poshta.Core.Events;
using Microsoft.Extensions.Logging;
using Poshta.Application.Interfaces.Services;
using Poshta.Core.Interfaces.Services;
using CSharpFunctionalExtensions;
using Poshta.Application.Auth;

namespace Poshta.Application.Services
{
    public class NotificationService(
        IUserService userService,
        ISmsService smsService,
        IEmailService emailService,
        IConfirmationCodeService confirmationCodeService,
        ILogger<NotificationService> logger) : INotificationService
    {
        private readonly IUserService userService = userService;
        private readonly ISmsService smsService = smsService;
        private readonly IEmailService emailService = emailService;
        private readonly IConfirmationCodeService confirmationCodeService = confirmationCodeService;
        private readonly ILogger<NotificationService> logger = logger;

        public async Task<Result<string>> SmsSendConfirmationCodeAsync(string phone, bool isRegistration)
        {
            logger.LogInformation("Start sms send confirmation code");

            if (isRegistration)
            {
                var userResult = await userService.GetUserByPhoneAsync(phone);

                if (userResult.IsSuccess)
                {
                    logger.LogError("User with this phone has already exist");
                    return Result.Failure<string>("User with this phone has already exist");
                }
            }

            var code = await confirmationCodeService.GenerateCodeAsync(phone);

            await smsService.SendSms(new SmsNotificationEventArgs()
            {
                Phone = phone,
                Message = $"Your confirmation code: {code}"
            });

            logger.LogInformation($"Success send sms confirmation code to phone: {phone}");
            return Result.Success($"Success send sms confirmation code to phone: {phone}");
        }

        public async Task<Result<string>> EmailSendConfirmationCodeAsync(string email)
        {
            logger.LogInformation("Start email send confirmation code");

            var code = await confirmationCodeService.GenerateCodeAsync(email);

            await emailService.SendEmail(new EmailNotificationEventArgs()
            {
                Email = email,
                Subject = "Confirmation code from Poshta",
                Body = $"Your confirmation code: {code}"
            });

            logger.LogInformation($"Success send confirmation code to email: {email}");
            return Result.Success($"Success send email confirmation code to email: {email}");
        }

        public async Task<Result<string>> NotifyShipmentArrivedAsync(Guid userId, string message)
        {
            logger.LogInformation($"Start notify shipment arrived for user with id: {userId}");

            var userResult = await userService.GetUserByIdAsync(userId);

            if (userResult.IsFailure)
            {
                logger.LogError($"User with id {userId} wasn't found");
                return Result.Failure<string>(userResult.Error);
            }

            var user = userResult.Value;

            logger.LogInformation($"Start sms notify shipment arrived for user with id: {userId}");

            await SmsNotifyShipmentArrivedAsync(userId, user.PhoneNumber, message);

            logger.LogInformation($"Success sms notify shipment arrived for user with id: {userId}");

            if (user.Email != null)
            {
                logger.LogInformation($"Start email notify shipment arrived for user with id: {userId}");

                await EmailNotifyShipmentArrivedAsync(userId, user.Email, message);

                logger.LogInformation($"Success email notify shipment arrived for user with id: {userId}");
            }

            logger.LogInformation($"Success notify shipment arrived for user with id: {userId}");
            return Result.Success($"Success notify shipment arrived for user with id: {userId}");
        }

        private async Task SmsNotifyShipmentArrivedAsync(Guid userId, string phone, string message)
        {
            try
            {
                await smsService.SendSms(new SmsNotificationEventArgs
                {
                    Message = message,
                    Phone = phone,
                });
            }
            catch (Exception ex)
            {
                logger.LogError($"Can't send sms to user with id: {userId} \n\rMessage: {ex.Message}");
            }
        }

        private async Task EmailNotifyShipmentArrivedAsync(Guid userId, string email, string message)
        {
            try
            {
                await emailService.SendEmail(new EmailNotificationEventArgs
                {
                    Body = message,
                    Email = email,
                    Subject = "Shipment Arrived",
                });
            }
            catch (Exception ex)
            {
                logger.LogError($"Can't send email to user with id: {userId} \n\rMessage: {ex.Message}");
            }
        }
    }
}
