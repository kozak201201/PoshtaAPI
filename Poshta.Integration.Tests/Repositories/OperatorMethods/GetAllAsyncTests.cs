using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.OperatorMethods
{
    public class GetAllAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOperators_WhenOperatorsExist()
        {
            // Arrange
            var user1 = await CreateTestUser1Async();
            var postOffice1 = await CreateTestPostOffice1Async();
            var user2 = await CreateTestUser2Async();
            var postOffice2 = await CreateTestPostOffice2Async();

            var operator1Result = Operator.Create(Guid.NewGuid(), user1.Id, postOffice1.Id);
            Assert.True(operator1Result.IsSuccess);
            await operatorsRepository.CreateAsync(operator1Result.Value);

            var operator2Result = Operator.Create(Guid.NewGuid(), user2.Id, postOffice2.Id);
            Assert.True(operator2Result.IsSuccess);
            await operatorsRepository.CreateAsync(operator2Result.Value);

            // Act
            var operators = await operatorsRepository.GetAllAsync();

            // Assert
            Assert.NotNull(operators);
            Assert.Equal(2, operators.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoOperatorsExist()
        {
            // Act
            var operators = await operatorsRepository.GetAllAsync();

            // Assert
            Assert.NotNull(operators);
            Assert.Empty(operators);
        }

        [Fact]
        public async Task GetAllAsync_ShouldIncludeRatings_WhenOperatorsExist()
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
            await operatorsRepository.AddRatingAsync(ratingResult.Value);

            // Act
            var operators = await operatorsRepository.GetAllAsync();

            // Assert
            Assert.NotNull(operators);
            Assert.Single(operators);
            Assert.Equal(createdOperator.UserId, operators.First().UserId);
            Assert.NotEmpty(operators.First().Ratings);
            Assert.Equal(ratingResult.Value.Rating, operators.First().Ratings[0].Rating);
            Assert.Equal(ratingResult.Value.Review, operators.First().Ratings[0].Review);
        }
    }
}
