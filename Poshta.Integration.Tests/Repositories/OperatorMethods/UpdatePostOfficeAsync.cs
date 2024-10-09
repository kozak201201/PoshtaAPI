using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.OperatorMethods
{
    public class UpdatePostOfficeAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task UpdatePostOfficeAsync_ShouldUpdatePostOfficeId_WhenOperatorExists()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var postOffice1 = await CreateTestPostOffice1Async();
            var postOffice2 = await CreateTestPostOffice2Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), user.Id, postOffice1.Id);
            Assert.True(operatorResult.IsSuccess);
            var createdOperator = operatorResult.Value;

            await operatorsRepository.CreateAsync(createdOperator);

            // Act
            await operatorsRepository.UpdatePostOfficeAsync(createdOperator.Id, postOffice2.Id);

            // Assert
            var updatedOperator = await operatorsRepository.GetByIdAsync(createdOperator.Id);
            Assert.NotNull(updatedOperator);
            Assert.Equal(postOffice2.Id, updatedOperator.PostOfficeId);
        }

        [Fact]
        public async Task UpdatePostOfficeAsync_ShouldThrowException_WhenOperatorDoesNotExist()
        {
            // Arrange
            var nonExistentOperatorId = Guid.NewGuid();
            var newPostOfficeId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                operatorsRepository.UpdatePostOfficeAsync(nonExistentOperatorId, newPostOfficeId));
            Assert.Contains($"Operator with id: {nonExistentOperatorId} wasn't found", exception.Message);
        }
    }
}
