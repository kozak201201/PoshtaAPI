using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.OperatorMethods
{
    public class DeleteAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task DeleteAsync_ShouldRemoveOperator_WhenOperatorExists()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var postOffice = await CreateTestPostOffice1Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), user.Id, postOffice.Id);
            Assert.True(operatorResult.IsSuccess);
            var createdOperator = operatorResult.Value;

            await operatorsRepository.CreateAsync(createdOperator);

            // Act
            await operatorsRepository.DeleteAsync(createdOperator.Id);

            // Assert
            var deletedOperator = await operatorsRepository.GetByIdAsync(createdOperator.Id);
            Assert.Null(deletedOperator);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenOperatorDoesNotExist()
        {
            // Arrange
            var nonExistentOperatorId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                operatorsRepository.DeleteAsync(nonExistentOperatorId));
            Assert.Contains($"Operator with id: {nonExistentOperatorId} wasn't found", exception.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUserRole_WhenOperatorDeleted()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var postOffice = await CreateTestPostOffice1Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), user.Id, postOffice.Id);
            Assert.True(operatorResult.IsSuccess);
            var createdOperator = operatorResult.Value;

            await operatorsRepository.CreateAsync(createdOperator);
            await usersRepository.AddRoleAsync(user.Id, "Operator");

            // Act
            await operatorsRepository.DeleteAsync(createdOperator.Id);

            // Assert
            var userRoles = await usersRepository.GetRolesAsync(user.Id);
            Assert.DoesNotContain("Operator", userRoles);
        }
    }
}
