using CSharpFunctionalExtensions;

namespace Poshta.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<Result<string>> CreatePaymentRequestUrlAsync(Guid shipmentId, Guid userId);
        Task<Result<string>> ProcessPaymentResult(Dictionary<string, string> parameters);
    }
}