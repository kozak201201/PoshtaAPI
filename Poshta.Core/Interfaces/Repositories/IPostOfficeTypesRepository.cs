using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Repositories
{
    public interface IPostOfficeTypesRepository
    {
        Task CreateAsync(PostOfficeType postOfficeType);
        Task DeleteAsync(Guid postOfficeTypeId);
        Task<PostOfficeType?> GetPostOfficeTypeByIdAsync(Guid postOfficeTypeId);
        Task<IEnumerable<PostOfficeType>> GetPostOfficeTypesAsync();
    }
}