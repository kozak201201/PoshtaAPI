using Poshta.Core.Models;

namespace Poshta.DataAccess.SqlServer.Entities
{
    public class ShipmentHistoryEntity
    {
        public Guid Id { get; set; }

        public Guid ShipmentId { get; set; }

        public required ShipmentEntity Shipment { get; set; }

        public ShipmentStatus Status { get; set; }

        public DateTime StatusDate { get; set; }

        public Guid PostOfficeId { get; set; }

        public required PostOfficeEntity PostOffice { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
