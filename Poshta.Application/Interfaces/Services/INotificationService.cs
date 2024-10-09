using CSharpFunctionalExtensions;

namespace Poshta.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<Result<string>> SmsSendConfirmationCodeAsync(string phone, bool isRegistration);
        Task<Result<string>> EmailSendConfirmationCodeAsync(string email);
        Task<Result<string>> NotifyShipmentArrivedAsync(Guid userId, string message);
    }
}