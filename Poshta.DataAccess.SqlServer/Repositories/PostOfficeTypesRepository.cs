using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Models;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Repositories
{
    public class PostOfficeTypesRepository(ApplicationDbContext context, IMapper mapper) : IPostOfficeTypesRepository
    {
        private readonly IMapper mapper = mapper;
        private readonly ApplicationDbContext context = context;

        public async Task CreateAsync(PostOfficeType postOfficeType)
        {
            var postOfficeTypeEntity = mapper.Map<PostOfficeType, PostOfficeTypeEntity>(postOfficeType);

            await context.PostOfficeTypes.AddAsync(postOfficeTypeEntity);
            await context.SaveChangesAsync();
        }

        public async Task<PostOfficeType?> GetPostOfficeTypeByIdAsync(Guid postOfficeTypeId)
        {
            var postOfficeType = await context.PostOfficeTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(pType => pType.Id == postOfficeTypeId);

            return postOfficeType != null
                ? mapper.Map<PostOfficeTypeEntity, PostOfficeType>(postOfficeType)
                : null;
        }

        public async Task<IEnumerable<PostOfficeType>> GetPostOfficeTypesAsync()
        {
            var postOfficeTypeEnities = await context.PostOfficeTypes.AsNoTracking().ToListAsync();

            return mapper.Map<List<PostOfficeTypeEntity>, List<PostOfficeType>>(postOfficeTypeEnities);
        }

        public async Task DeleteAsync(Guid postOfficeTypeId)
        {
            var postOfficeTypeEntity = await context.PostOfficeTypes.FindAsync(postOfficeTypeId)
                ?? throw new Exception($"Post office with id: {postOfficeTypeId} wasn't found");

            context.PostOfficeTypes.Remove(postOfficeTypeEntity);
            await context.SaveChangesAsync();
        }
    }
}
