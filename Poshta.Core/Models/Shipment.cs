using CSharpFunctionalExtensions;

namespace Poshta.Core.Models
{
    public enum ShipmentStatus
    {
        Created = 1,
        AtPostOffice,
        InTransit,
        Delivered,
        Redirected,
        Received
    }

    public enum PayerType
    {
        Sender = 1,
        Recipient
    }

    public class Shipment
    {
        public const double MIN_APPRAISED_VALUE = 100;
        public const double MAX_APPRAISED_VALUE = 30_000;

        public const double MIN_PRICE = 40;

        public const float MIN_WEIGHT = 0.1f;
        public const float DEFAULT_WEIGHT = 1;

        public const float MIN_LENGTH = 16;
        public const float MIN_WIDTH = 11;
        public const float MIN_HEIGHT = 10;

        public const int TRACKING_NUMBER_LENGTH = 14;

        private readonly List<ShipmentHistory> shipmentHistories = [];

        private Shipment(
            Guid id,
            Guid senderId,
            Guid recipientId,
            Guid startPostOfficeId,
            Guid endPostOfficeId,
            PayerType payer,
            string trackingNumber,
            double price,
            double appraisedValue,
            float weight,
            float length,
            float width,
            float height)
        {
            Id = id;
            TrackingNumber = trackingNumber;
            SenderId = senderId;
            RecipientId = recipientId;
            StartPostOfficeId = startPostOfficeId;
            EndPostOfficeId = endPostOfficeId;
            Payer = payer;
            Price = price;
            AppraisedValue = appraisedValue;
            Weight = weight;
            Length = length;
            Width = width;
            Height = height;
            Status = ShipmentStatus.Created;
        }

        public Guid Id { get; }

        public Guid SenderId { get; }

        public Guid RecipientId { get; }

        public Guid? ConfidantId { get; set; }

        public Guid StartPostOfficeId { get; }

        public Guid EndPostOfficeId { get; }

        public Guid? CurrentPostOfficeId { get; set; }

        public Guid? OperatorWhoIssuedId { get; set; }

        public ShipmentStatus Status { get; set; }

        public PayerType Payer { get; }

        public string TrackingNumber { get; }

        public bool IsPaid { get; set; }

        public bool IsDeletedBySender { get; set; }

        public bool IsDeletedByRecipient { get; set; }

        public bool IsDeletedByConfidant { get; set; }

        public float Weight { get; }

        public float Length { get; }

        public float Width { get; }

        public float Height { get; }

        public double AppraisedValue { get; }

        public double Price { get; }

        public IReadOnlyList<ShipmentHistory> ShipmentHistories => shipmentHistories;

        public static Result<Shipment> Create(
            Guid id,
            Guid senderId,
            Guid recipientId,
            Guid startPostOfficeId,
            Guid endPostOfficeId,
            PayerType payer,
            string trackingNumber,
            double price,
            double appraisedValue = MIN_APPRAISED_VALUE,
            float weight = DEFAULT_WEIGHT,
            float length = MIN_LENGTH,
            float width = MIN_WIDTH,
            float height = MIN_HEIGHT)
        {
            if (appraisedValue < MIN_APPRAISED_VALUE)
                return Result.Failure<Shipment>($"appraised value cannot be less then {MIN_APPRAISED_VALUE}");

            if (price < MIN_PRICE)
                return Result.Failure<Shipment>($"min price cannot be less then {MIN_PRICE}");

            if (weight < MIN_WEIGHT)
                return Result.Failure<Shipment>($"weight cannot be less then {MIN_WEIGHT}");

            if (length < MIN_LENGTH)
                return Result.Failure<Shipment>($"length cannot be less then {MIN_LENGTH}");

            if (width < MIN_WIDTH)
                return Result.Failure<Shipment>($"width cannot be less then {MIN_WIDTH}");

            if (height < MIN_HEIGHT)
                return Result.Failure<Shipment>($"height value cannot be less then {MIN_HEIGHT}");

            return new Shipment(
                id,
                senderId,
                recipientId,
                startPostOfficeId,
                endPostOfficeId,
                payer,
                trackingNumber,
                price,
                appraisedValue,
                weight,
                length,
                width,
                height);
        }
    }
}
