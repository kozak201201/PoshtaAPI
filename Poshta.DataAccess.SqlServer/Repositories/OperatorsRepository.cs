using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Poshta.Core.Exceptions;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Models;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Repositories
{
    public class OperatorsRepository(
        ApplicationDbContext context,
        UserManager<UserEntity> userManager, 
        IMapper mapper) : IOperatorsRepository
    {
        private readonly IMapper mapper = mapper;
        private readonly ApplicationDbContext context = context;

        public async Task CreateAsync(Operator operatorPostOffice)
        {
            var operatorPostOfficeEntity = mapper.Map<Operator, OperatorEntity>(operatorPostOffice);

            var operatorWithSameUserId = await context.Operators.FirstOrDefaultAsync(op => op.UserId == operatorPostOffice.UserId);

            if (operatorWithSameUserId != null)
            {
                throw new UserIsAlreadyOperatorToThePostOfficeException(
                    operatorPostOffice.UserId,
                    operatorPostOffice.PostOfficeId);
            }

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var user = await userManager.FindByIdAsync(operatorPostOffice.UserId.ToString());

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                await context.Operators.AddAsync(operatorPostOfficeEntity);
                await context.SaveChangesAsync();

                var isInRole = await userManager.IsInRoleAsync(user, "Operator");

                if (!isInRole)
                {
                    var result = await userManager.AddToRoleAsync(user, "Operator");
                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to assign 'Operator' role to user");
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Operator?> GetByIdAsync(Guid operatorId)
        {
            var operatorEntity = await context.Operators
                .AsNoTracking()
                .Include(o => o.Ratings)
                .FirstOrDefaultAsync(o => o.Id == operatorId);

            var operatorPostOffice = mapper.Map<OperatorEntity, Operator>(operatorEntity!);
            return operatorPostOffice;
        }

        public async Task<Operator?> GetByUserIdAsync(Guid userId)
        {
            var operatorEntity = await context.Operators
                .AsNoTracking()
                .Include(o => o.Ratings)
                .FirstOrDefaultAsync(o => o.UserId == userId);

            var operatorPostOffice = mapper.Map<OperatorEntity, Operator>(operatorEntity!);
            return operatorPostOffice;
        }

        public async Task<IEnumerable<Operator>> GetAllAsync()
        {
            var operatorsEntity = await context.Operators
                .AsNoTracking()
                .Include(o => o.Ratings)
                .ToListAsync();

            var operators = mapper.Map<List<OperatorEntity>, List<Operator>>(operatorsEntity!);
            return operators;
        }

        public async Task UpdatePostOfficeAsync(Guid operatorId, Guid newPostOfficeId)
        {
            var operatorEntity = await context.Operators.FindAsync(operatorId)
                ?? throw new Exception($"Operator with id: {operatorId} wasn't found");

            operatorEntity.PostOfficeId = newPostOfficeId;
            await context.SaveChangesAsync();
        }

        public async Task AddRatingAsync(OperatorRating rating)
        {
            var operatorRatingEntity = mapper.Map<OperatorRating, OperatorRatingEntity>(rating);

            await context.OperatorRatings.AddAsync(operatorRatingEntity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid operatorId)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var operatorEntity = await context.Operators.FindAsync(operatorId)
                    ?? throw new Exception($"Operator with id: {operatorId} wasn't found");

                context.Operators.Remove(operatorEntity);
                await context.SaveChangesAsync();

                var user = await userManager.FindByIdAsync(operatorEntity.UserId.ToString());

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                if (await userManager.IsInRoleAsync(user, "Operator"))
                {
                    var result = await userManager.RemoveFromRoleAsync(user, "Operator");

                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to remove 'Operator' role from user");
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
