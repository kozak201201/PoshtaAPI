using CSharpFunctionalExtensions;

namespace Poshta.Core.Models
{
    public class Operator
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid PostOfficeId { get; set; }

        private readonly List<OperatorRating> ratings = [];

        public IReadOnlyList<OperatorRating> Ratings => ratings;

        private Operator(Guid id, Guid userId, Guid postOfficeId)
        {
            Id = id;
            UserId = userId;
            PostOfficeId = postOfficeId;
        }

        public static Result<Operator> Create(Guid id, Guid userId, Guid postOfficeId)
        {
            return new Operator(id, userId, postOfficeId); 
        }
    }
}
