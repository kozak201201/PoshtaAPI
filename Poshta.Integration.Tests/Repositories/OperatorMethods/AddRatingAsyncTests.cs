using Microsoft.EntityFrameworkCore;
using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.OperatorMethods
{
    public class AddRatingAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task AddRatingAsync_ShouldAddRating_WhenRatingIsValid()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var postOffice = await CreateTestPostOffice1Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), user.Id, postOffice.Id);
            Assert.True(operatorResult.IsSuccess);
            var createdOperator = operatorResult.Value;

            await operatorsRepository.CreateAsync(createdOperator);

            var ratingResult = OperatorRating.Create(Guid.NewGuid(), createdOperator.Id, user.Id, 5, "Nice!", DateTime.UtcNow);
            Assert.True(ratingResult.IsSuccess);

            // Act
            await operatorsRepository.AddRatingAsync(ratingResult.Value);

            // Assert
            var updatedOperator = await operatorsRepository.GetByIdAsync(createdOperator.Id);
            Assert.NotNull(updatedOperator);
            Assert.NotEmpty(updatedOperator.Ratings);
            Assert.Equal(ratingResult.Value.Rating, updatedOperator.Ratings[0].Rating);
            Assert.Equal(ratingResult.Value.Review, updatedOperator.Ratings[0].Review);
        }

        [Fact]
        public async Task AddRatingAsync_ShouldThrowException_WhenOperatorDoesNotExist()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var invalidOperatorId = Guid.NewGuid();

            var ratingResult = OperatorRating.Create(Guid.NewGuid(), invalidOperatorId, user.Id, 5, "Nice!", DateTime.UtcNow);
            Assert.True(ratingResult.IsSuccess);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
                operatorsRepository.AddRatingAsync(ratingResult.Value));
        }
    }
}
