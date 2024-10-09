using CSharpFunctionalExtensions;
using Poshta.Core.Models;

namespace Poshta.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<string>> LoginAsync(string phone, string password);

        Task<Result<string>> RegisterAsync(
            string lastName,
            string firstName,
            string password,
            string phoneNuber,
            string confirmationCode,
            string? middlename = null);

        Task<IEnumerable<User>> GetUsersAsync();

        Task<Result<User>> GetUserByPhoneAsync(string phone);

        Task<Result<User>> GetUserByIdAsync(Guid userId);

        Task<Result<string>> UpdateEmailAsync(Guid userId, string email, string confirmationCode);

        Task<Result<string>> AddRoleAsync(Guid userId, string role);

        Task<Result<string>> RemoveRoleAsync(Guid userId, string role);

        Task<Result<string>> DeleteUserAsync(Guid userId);
    }
}
