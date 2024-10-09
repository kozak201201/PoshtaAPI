using CSharpFunctionalExtensions;
using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Services
{
    public interface IPostOfficeService
    {
        Task<Result<PostOffice>> CreateAsync(int number, string city, string address, 
            int maxShipmentsCount, double latitude, double longitude, Guid postOfficeTypeId);
        Task<Result<PostOffice>> GetPostOfficeByIdAsync(Guid postOfficeId);
        Task<IEnumerable<PostOffice>> GetPostOfficesAsync();
        Task<Result> DeleteAsync(Guid postOfficeId);
    }
}