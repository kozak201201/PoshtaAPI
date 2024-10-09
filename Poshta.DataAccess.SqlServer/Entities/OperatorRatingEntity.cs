namespace Poshta.DataAccess.SqlServer.Entities
{
    public class OperatorRatingEntity
    {
        public Guid Id { get; set; }

        public Guid OperatorId { get; set; }

        public OperatorEntity? Operator { get; set; }

        public Guid UserId { get; set; }

        public UserEntity? User { get; set; }

        public int Rating { get; set; }

        public string Review { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
