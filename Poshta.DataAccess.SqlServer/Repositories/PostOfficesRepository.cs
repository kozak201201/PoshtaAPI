using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Poshta.Core.Exceptions;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Models;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Repositories
{
    public class PostOfficesRepository(ApplicationDbContext context, IMapper mapper) : IPostOfficesRepository
    {
        private readonly IMapper mapper = mapper;
        private readonly ApplicationDbContext context = context;

        public async Task CreateAsync(PostOffice postOffice)
        {
            var postOfficeEntity = mapper.Map<PostOffice, PostOfficeEntity>(postOffice);

            var postOfficeTypeEntity = await context.PostOfficeTypes.FindAsync(postOffice.Type.Id);

            if (postOfficeTypeEntity == null)
            {
                throw new Exception("PostOfficeType not found");
            }

            postOfficeEntity.Type = postOfficeTypeEntity;

            var existsWithSameCoordinates = await context.PostOffices
                .AnyAsync(po => po.Latitude == postOfficeEntity.Latitude && po.Longitude == postOfficeEntity.Longitude);

            if (existsWithSameCoordinates)
            {
                throw new CoordinatesAlreadyExistException(postOffice.Latitude, postOffice.Longitude);
            }

            var existsWithSameNumberAddressCity = await context.PostOffices
                .AnyAsync(po => po.Number == postOfficeEntity.Number && 
                po.City == postOfficeEntity.City && 
                po.Address == postOfficeEntity.Address);

            if (existsWithSameNumberAddressCity)
            {
                throw new NumberAddressCityAlreadyExistException(postOffice.Number, postOffice.Address, postOffice.City);
            }

            var result = await context.PostOffices.AddAsync(postOfficeEntity);
            await context.SaveChangesAsync();
        }

        public async Task<PostOffice?> GetPostOfficeByIdAsync(Guid postOfficeId)
        {
            var postOffice = await context.PostOffices
                .AsNoTracking()
                .Include(p => p.Type)
                .Include(p => p.Operators)
                .Include(p => p.Shipments)
                .FirstOrDefaultAsync(po => po.Id == postOfficeId);

            return postOffice != null ? mapper.Map<PostOfficeEntity, PostOffice>(postOffice) : null;
        }

        public async Task<IEnumerable<PostOffice>> GetPostOfficesAsync()
        {
            var postOfficeEnities = await context.PostOffices.AsNoTracking()
                .Include(p => p.Type)
                .Include(p => p.Operators)
                .ToListAsync();

            return mapper.Map<List<PostOfficeEntity>, List<PostOffice>>(postOfficeEnities);
        }

        public async Task DeleteAsync(Guid postOfficeId)
        {
            var postOfficeEntity = await context.PostOffices.FindAsync(postOfficeId)
                ?? throw new Exception($"Post office with id: {postOfficeId} wasn't found");

            context.PostOffices.Remove(postOfficeEntity);
             await context.SaveChangesAsync();
        }
    }
}
