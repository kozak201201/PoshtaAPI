using Poshta.Core.Models;

namespace Poshta.DataAccess.SqlServer.Entities
{
    public class ShipmentEntity
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }

        public UserEntity? Sender { get; set; }

        public Guid RecipientId { get; set; }

        public UserEntity? Recipient { get; set; }

        public Guid? ConfidantId { get; set; }

        public UserEntity? Confidant { get; set; }

        public Guid StartPostOfficeId { get; set; }

        public PostOfficeEntity? StartPostOffice { get; set; }

        public Guid EndPostOfficeId { get; set; }

        public PostOfficeEntity? EndPostOffice { get; set; }

        public Guid? CurrentPostOfficeId { get; set; }

        public PostOfficeEntity? CurrentPostOffice { get; set; }

        public Guid? OperatorWhoIssuedId { get; set; }

        public OperatorEntity? OperatorWhoIssued { get; set; }

        public ShipmentStatus Status { get; set; }

        public PayerType Payer { get; set; }

        public string? TrackingNumber { get; set; }

        public bool IsPaid { get; set; }

        public bool IsDeletedBySender { get; set; }

        public bool IsDeletedByRecipient { get; set; }

        public bool IsDeletedByConfidant { get; set; }

        public float Weight { get; set; }

        public float Length { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public double AppraisedValue { get; set; }

        public double Price { get; set; }

        public List<ShipmentHistoryEntity> ShipmentHistories { get; set; } = [];
    }
}
