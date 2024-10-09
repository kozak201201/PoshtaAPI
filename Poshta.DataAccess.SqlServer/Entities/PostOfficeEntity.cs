namespace Poshta.DataAccess.SqlServer.Entities
{
    public class PostOfficeEntity
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int MaxShipmentsCount { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public Guid TypeId { get; set; }

        public PostOfficeTypeEntity? Type { get; set; }

        public List<ShipmentEntity> Shipments { get; set; } = [];

        public List<OperatorEntity> Operators { get; set; } = [];
    }
}
