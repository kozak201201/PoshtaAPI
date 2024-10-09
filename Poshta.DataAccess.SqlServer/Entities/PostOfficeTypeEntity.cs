namespace Poshta.DataAccess.SqlServer.Entities
{
    public class PostOfficeTypeEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public float MaxShipmentWeight { get; set; }

        public float MaxShipmentLength { get; set; }

        public float MaxShipmentWidth { get; set; }

        public float MaxShipmentHeight { get; set; }
    }
}
