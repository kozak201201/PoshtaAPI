using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Poshta.Core.Exceptions;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Models;
using Poshta.DataAccess.SqlServer.Entities;
using System.Data;

namespace Poshta.DataAccess.SqlServer.Repositories
{
    public class UsersRepository(
        ApplicationDbContext context,
        UserManager<UserEntity> userManager,
        IMapper mapper) : IUsersRepository
    {
        private readonly ApplicationDbContext context = context;
        private readonly UserManager<UserEntity> userManager = userManager;
        private readonly IMapper mapper = mapper;

        public async Task<Result> CreateAsync(User user)
        {
            var userEntity = mapper.Map<UserEntity>(user);

            var existsWithSamePhoneNumber = await context.Users
                .AnyAsync(u => u.PhoneNumber == user.PhoneNumber);

            if (existsWithSamePhoneNumber)
            {
                throw new PhoneAlreadyExistException(user.PhoneNumber);
            }

            userEntity.UserName = user.Id.ToString();
            userEntity.PhoneNumberConfirmed = true;

            var result = await userManager.CreateAsync(userEntity, user.PasswordHash);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var userEntity = await context.Users.FindAsync(id);

            return mapper.Map<UserEntity?, User>(userEntity);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var userEntities = await context.Users.AsNoTracking().ToListAsync();

            return mapper.Map<List<UserEntity>, List<User>>(userEntities);
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            var userEntity = await context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);

            return mapper.Map<UserEntity?, User>(userEntity);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            return mapper.Map<UserEntity?, User>(userEntity);
        }

        public async Task UpdateNameAsync(Guid userId, string firstName, string lastName, string middleName)
        {
            var userEntity = await context.Users.FindAsync(userId)
                ?? throw new Exception($"User with id: {userId} wasn't found");

            userEntity.FirstName = firstName;
            userEntity.LastName = lastName;
            userEntity.MiddleName = middleName;

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetRolesAsync(Guid userId)
        {
            var userEntity = await context.Users.FindAsync(userId)
                ?? throw new Exception($"User with id: {userId} wasn't found");

            var roles = await userManager.GetRolesAsync(userEntity);
            return roles;
        }

        public async Task<bool> CheckPasswordAsync(Guid userId, string password)
        {
            var userEntity = await context.Users.FindAsync(userId)
                ?? throw new Exception($"User with id: {userId} wasn't found");

            return await userManager.CheckPasswordAsync(userEntity, password);
        }

        public async Task UpdateEmailAsync(Guid userId, string email)
        {
            var userEntity = await context.Users.FindAsync(userId)
                ?? throw new Exception($"User with id: {userId} wasn't found");

            var existsWithSameEmail = await context.Users
                .AnyAsync(u => u.Email == userEntity.Email && u.Id != userEntity.Id);

            if (existsWithSameEmail)
            {
                throw new EmailAlreadyExistException(email);
            }

            userEntity.Email = email;
            userEntity.NormalizedEmail = email.ToUpper();
            userEntity.EmailConfirmed = true;

            await context.SaveChangesAsync();
        }

        public async Task<Result> AddRoleAsync(Guid userId, string role)
        {
            var userEntity = await context.Users.FindAsync(userId)
                ?? throw new Exception($"User with id: {userId} wasn't found");

            var result = await userManager.AddToRoleAsync(userEntity, role);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<Result> RemoveRoleAsync(Guid userId, string role)
        {
            var userEntity = await context.Users.FindAsync(userId)
                ?? throw new Exception($"User with id: {userId} wasn't found");

            var result = await userManager.RemoveFromRoleAsync(userEntity, role);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<Result> DeleteAsync(Guid userId)
        {
            var userEntity = await context.Users.FindAsync(userId)
                ?? throw new Exception($"User with id: {userId} wasn't found");

            var result = await userManager.DeleteAsync(userEntity);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
