using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Models;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Repositories
{
    public class ShipmentsRepository(ApplicationDbContext context, IMapper mapper) : IShipmentsRepository
    {
        private readonly IMapper mapper = mapper;
        private readonly ApplicationDbContext context = context;

        public async Task CreateAsync(Shipment shipment)
        {
            var users = await context.Users
                .Where(u => u.Id == shipment.SenderId || u.Id == shipment.RecipientId)
                .ToListAsync();

            var shipmentEntity = await context.Shipments.FindAsync(shipment.Id);

            shipmentEntity = mapper.Map<Shipment, ShipmentEntity>(shipment);

            await context.Shipments.AddAsync(shipmentEntity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Shipment>> GetByUserIdAsync(Guid userId)
        {
            var shipmentEntities = await context.Shipments
                .AsNoTracking()
                .Include(s => s.ShipmentHistories)
                .Where(s => s.SenderId == userId || s.RecipientId == userId || 
                    (s.ConfidantId.HasValue && s.ConfidantId == userId))
                .ToListAsync();

            return mapper.Map<List<ShipmentEntity>, List<Shipment>>(shipmentEntities);
        }

        public async Task<IEnumerable<Shipment>> GetAllAsync()
        {
            var shipmentEntities = await context.Shipments
                .AsNoTracking()
                .Include(s => s.ShipmentHistories)
                .ToListAsync();

            return mapper.Map<List<ShipmentEntity>, List<Shipment>>(shipmentEntities);
        }

        public async Task<IEnumerable<Shipment>> GetByPostOfficeIdAsync(Guid postOfficeId)
        {
            var shipmentEntities = await context.Shipments
                .AsNoTracking()
                .Include(s => s.ShipmentHistories)
                .Where(s => s.CurrentPostOfficeId == postOfficeId)
                .ToListAsync();

            return mapper.Map<List<ShipmentEntity>, List<Shipment>>(shipmentEntities);
        }

        public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
        {
            var shipmentEntity = await context.Shipments
                .AsNoTracking()
                .Include(s => s.ShipmentHistories)
                .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);

            var shipment = mapper.Map<ShipmentEntity, Shipment>(shipmentEntity!);
            return shipment;
        }

        public async Task<Shipment?> GetByIdAsync(Guid shipmentId)
        {
            var shipmentEntity = await context.Shipments
                .AsNoTracking()
                .Include(s => s.ShipmentHistories)
                .FirstOrDefaultAsync(s => s.Id == shipmentId);

            var shipment = mapper.Map<ShipmentEntity, Shipment>(shipmentEntity!);
            return shipment;
        }

        public async Task UpdatePaidStatusAsync(Guid shipmentId, bool newPaidStatus)
        {
            await context.Shipments
                .Where(s => s.Id == shipmentId)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.IsPaid, newPaidStatus));
        }

        public async Task UpdateConfidantAsync(Guid shipmentId, Guid? confidantId)
        {
            var shipmentEntity = await context.Shipments
                .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (shipmentEntity == null)
            {
                throw new Exception($"Shipment with id: {shipmentId} wasn't found");
            }

            shipmentEntity!.ConfidantId = confidantId;

            await context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(Guid shipmentId, ShipmentStatus status)
        {
            await context.Shipments
                .Where(s => s.Id == shipmentId)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.Status, status));
        }

        public async Task UpdateEndPostOfficeAsync(Guid shipmentId, Guid newEndPostOfficeId)
        {
            await context.Shipments
                .Where(s => s.Id == shipmentId)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.EndPostOfficeId, newEndPostOfficeId));
        }

        public async Task AddHistoryAsync(Guid shipmentId, ShipmentHistory shipmentHistory)
        {
            var shipmentHistoryEntity = mapper.Map<ShipmentHistory, ShipmentHistoryEntity>(shipmentHistory);

            await context.ShipmentHistories.AddAsync(shipmentHistoryEntity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCurrentPostOfficeAsync(Guid shipmentId, Guid? currentPostOffice)
        {
            await context.Shipments
                .Where(s => s.Id == shipmentId)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.CurrentPostOfficeId, currentPostOffice));
        }

        public async Task AddOperatorWhoIssuedId(Guid shipmentId, Guid operatorId)
        {
            await context.Shipments
                .Where(s => s.Id == shipmentId)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.OperatorWhoIssuedId, operatorId));
        }

        public async Task SoftDeleteAsync(Guid shipmentId, Guid userId)
        {
            var shipmentEntity = await context.Shipments
                .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (shipmentEntity!.SenderId == userId)
            {
                shipmentEntity.IsDeletedBySender = true;
            }
            else if (shipmentEntity.RecipientId == userId)
            {
                shipmentEntity.IsDeletedByRecipient = true;
            }
            else if (shipmentEntity.ConfidantId.HasValue && shipmentEntity.ConfidantId.Value == userId)
            {
                shipmentEntity.IsDeletedByConfidant = true;
            }

            if (shipmentEntity.IsDeletedBySender && shipmentEntity.IsDeletedByRecipient &&
                (!shipmentEntity.ConfidantId.HasValue || shipmentEntity.IsDeletedByConfidant))
            {
                context.Shipments.Remove(shipmentEntity);
            }
            
            await context.SaveChangesAsync();
        }
    }
}
