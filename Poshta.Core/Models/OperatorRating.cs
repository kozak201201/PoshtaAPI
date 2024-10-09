using CSharpFunctionalExtensions;

namespace Poshta.Core.Models
{
    public class OperatorRating
    {
        public const int MIN_RATING = 1;
        public const int MAX_RATING = 5;

        public Guid Id { get; set; }

        public Guid OperatorId { get; set; }

        public Guid UserId { get; set; }

        public int Rating { get; set; }

        public string Review { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        private OperatorRating(
            Guid id,
            Guid operatorId,
            Guid userId,
            int rating,
            string review,
            DateTime createdAt)
        {
            Id = id;
            OperatorId = operatorId;
            UserId = userId;
            Rating = rating;
            Review = review;
            CreatedAt = createdAt;
        }

        public static Result<OperatorRating> Create(
            Guid id,
            Guid operatorId,
            Guid userId,
            int rating,
            string review,
            DateTime createdAt)
        {
            if (rating < MIN_RATING || rating > MAX_RATING)
            {
                return Result.Failure<OperatorRating>($"Rating can't be " +
                    $"less than {MIN_RATING} or more then {MAX_RATING}");
            }

            return new OperatorRating(
                id,
                operatorId,
                userId,
                rating,
                review,
                createdAt);
        }
    }
}
