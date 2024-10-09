using CSharpFunctionalExtensions;

namespace Poshta.Core.Models
{
    public class ShipmentHistory
    {
        private ShipmentHistory(
            Guid id,
            Guid shipmentId,
            ShipmentStatus shipmentStatus,
            Guid postOfficeId,
            DateTime statusDate,
            string desctiption
            )
        {
            Id = id;
            ShipmentId = shipmentId;
            Status = shipmentStatus;
            PostOfficeId = postOfficeId;
            Description = desctiption;
            StatusDate = statusDate;
        }

        public Guid Id { get; }

        public Guid ShipmentId { get; }

        public ShipmentStatus Status { get; }

        public DateTime StatusDate { get; }

        public Guid PostOfficeId { get; }

        public string Description { get; }

        public static Result<ShipmentHistory> Create(
            Guid id,
            Guid shipmentId,
            ShipmentStatus shipmentStatus,
            Guid postOfficeId,
            DateTime statusDate,
            string description)
        {
            if (string.IsNullOrEmpty(description))
                return Result.Failure<ShipmentHistory>("description can't be null or empty");

            return new ShipmentHistory(
                id,
                shipmentId,
                shipmentStatus,
                postOfficeId,
                statusDate,
                description);
        }
    }
}
