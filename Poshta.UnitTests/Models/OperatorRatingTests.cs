using Poshta.Core.Models;

namespace Poshta.UnitTests.Models
{
    public class OperatorRatingTests
    {
        [Fact]
        public void Create_ValidOperatorRating_ReturnsSuccessResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var rating = 5;
            var review = "Excellent service!";
            var createdAt = DateTime.UtcNow;

            // Act
            var result = OperatorRating.Create(id, operatorId, userId, rating, review, createdAt);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(id, result.Value.Id);
            Assert.Equal(operatorId, result.Value.OperatorId);
            Assert.Equal(userId, result.Value.UserId);
            Assert.Equal(rating, result.Value.Rating);
            Assert.Equal(review, result.Value.Review);
            Assert.Equal(createdAt, result.Value.CreatedAt);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void Create_InvalidRating_ReturnsFailureResult(int invalidRating)
        {
            // Arrange
            var id = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var review = "Some review";
            var createdAt = DateTime.UtcNow;

            // Act
            var result = OperatorRating.Create(id, operatorId, userId, invalidRating, review, createdAt);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Rating can't be less than 1 or more then 5", result.Error);
        }
    }
}
