using CSharpFunctionalExtensions;
using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Services
{
    public interface IPostOfficeTypeService
    {
        Task<Result<PostOfficeType>> CreateAsync(string name, 
            float maxShipmentWeight, 
            float maxShipmentLength, 
            float maxShipmentWidth, 
            float maxShipmentHeight);
        Task<Result<string>> DeleteByIdAsync(Guid postOfficeTypeId);
        Task<IEnumerable<PostOfficeType>> GetAllAsync();
        Task<Result<PostOfficeType>> GetByIdAsync(Guid postOfficeTypeId);
    }
}