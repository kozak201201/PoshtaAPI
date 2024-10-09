using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Repositories
{
    public interface IOperatorsRepository
    {
        Task AddRatingAsync(OperatorRating rating);
        Task CreateAsync(Operator operatorPostOffice);
        Task DeleteAsync(Guid operatorId);
        Task<Operator?> GetByIdAsync(Guid operatorId);
        Task<Operator?> GetByUserIdAsync(Guid userId);
        Task <IEnumerable<Operator>> GetAllAsync();
        Task UpdatePostOfficeAsync(Guid operatorId, Guid newPostOfficeId);
    }
}