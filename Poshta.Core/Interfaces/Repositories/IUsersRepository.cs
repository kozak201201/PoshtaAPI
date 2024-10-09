using CSharpFunctionalExtensions;
using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task<Result> CreateAsync(User user);

        Task<User?> GetByPhoneAsync(string phone);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(Guid id);

        Task<IEnumerable<User>> GetAllAsync();

        Task<bool> CheckPasswordAsync(Guid userId, string password);
        
        Task<IEnumerable<string>> GetRolesAsync(Guid userId);
        
        Task UpdateNameAsync(Guid id, string firstName, string lastName, string middleName);

        Task UpdateEmailAsync(Guid id, string email);

        Task<Result> AddRoleAsync(Guid userId, string role);
        
        Task<Result> RemoveRoleAsync(Guid userId, string role);

        Task<Result> DeleteAsync(Guid userId);
    }
}