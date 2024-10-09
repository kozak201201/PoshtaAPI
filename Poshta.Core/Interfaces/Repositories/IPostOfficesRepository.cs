using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Repositories
{
    public interface IPostOfficesRepository
    {
        Task CreateAsync(PostOffice postOffice);
        Task<PostOffice?> GetPostOfficeByIdAsync(Guid postOfficeId);
        Task<IEnumerable<PostOffice>> GetPostOfficesAsync();
        Task DeleteAsync(Guid postOfficeId);
    }
}