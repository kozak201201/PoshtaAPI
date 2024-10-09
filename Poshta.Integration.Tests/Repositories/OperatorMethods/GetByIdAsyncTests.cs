using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.OperatorMethods
{
    public class GetByIdAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnOperator_WhenOperatorExists()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var postOffice = await CreateTestPostOffice1Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), user.Id, postOffice.Id);
            Assert.True(operatorResult.IsSuccess);
            var operatorToCreate = operatorResult.Value;

            await operatorsRepository.CreateAsync(operatorToCreate);

            // Act
            var retrievedOperator = await operatorsRepository.GetByIdAsync(operatorToCreate.Id);

            // Assert
            Assert.NotNull(retrievedOperator);
            Assert.Equal(operatorToCreate.UserId, retrievedOperator.UserId);
            Assert.Equal(operatorToCreate.PostOfficeId, retrievedOperator.PostOfficeId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenOperatorDoesNotExist()
        {
            // Act
            var retrievedOperator = await operatorsRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(retrievedOperator);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldIncludeRatings_WhenOperatorExists()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var postOffice = await CreateTestPostOffice1Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), user.Id, postOffice.Id);
            Assert.True(operatorResult.IsSuccess);
            var operatorToCreate = operatorResult.Value;

            await operatorsRepository.CreateAsync(operatorToCreate);

            var ratingResult = OperatorRating.Create(Guid.NewGuid(), operatorToCreate.Id, user.Id, 5, "Nice!", DateTime.UtcNow);
            Assert.True(ratingResult.IsSuccess);
            await operatorsRepository.AddRatingAsync(ratingResult.Value);

            // Act
            var retrievedOperator = await operatorsRepository.GetByIdAsync(operatorToCreate.Id);

            // Assert
            Assert.NotNull(retrievedOperator);
            Assert.NotEmpty(retrievedOperator.Ratings);
            Assert.Single(retrievedOperator.Ratings);
            Assert.Equal(ratingResult.Value.Rating, retrievedOperator.Ratings[0].Rating);
            Assert.Equal(ratingResult.Value.Review, retrievedOperator.Ratings[0].Review);
        }
    }
}
