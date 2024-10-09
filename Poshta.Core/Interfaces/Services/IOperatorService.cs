using CSharpFunctionalExtensions;
using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Services
{
    public interface IOperatorService
    {
        Task<Result<string>> AddRatingAsync(Guid operatorId, Guid userId, Guid shipmentId, int rating, string review);
        Task<Result<Operator>> CreateAsync(Guid userId, Guid postOfficeId);
        Task<Result<string>> DeleteAsync(Guid operatorId);
        Task<Result<Operator>> GetByIdAsync(Guid operatorId);
        Task<Result<Operator>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Operator>> GetAllAsync();
        Task<Result<string>> UpdatePostOfficeAsync(Guid operatorId, Guid postOfficeId);
    }
}