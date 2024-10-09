namespace Poshta.DataAccess.SqlServer.Entities
{
    public class OperatorEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public UserEntity? User { get; set; }

        public Guid PostOfficeId { get; set; }

        public PostOfficeEntity? PostOffice { get; set; }

        public List<OperatorRatingEntity> Ratings { get; set; } = [];
    }
}
