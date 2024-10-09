using Poshta.Core.Exceptions;
using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.OperatorMethods
{
    public class CreateAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task CreateAsync_ShouldCreateOperator_WhenUserDoesNotExistAsOperator()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var postOffice = await CreateTestPostOffice1Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), user.Id, postOffice.Id);
            Assert.True(operatorResult.IsSuccess);
            var operatorToCreate = operatorResult.Value;

            // Act
            await operatorsRepository.CreateAsync(operatorToCreate);

            // Assert
            var createdOperator = await operatorsRepository.GetByUserIdAsync(user.Id);
            Assert.NotNull(createdOperator);
            Assert.Equal(user.Id, createdOperator.UserId);
            Assert.Equal(postOffice.Id, createdOperator.PostOfficeId);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowUserIsAlreadyOperatorToThePostOfficeException_WhenUserAlreadyExistsAsOperator()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var postOffice = await CreateTestPostOffice1Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), user.Id, postOffice.Id);
            Assert.True(operatorResult.IsSuccess);
            var operatorToCreate = operatorResult.Value;

            // First creation
            await operatorsRepository.CreateAsync(operatorToCreate);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserIsAlreadyOperatorToThePostOfficeException>(
                async () => await operatorsRepository.CreateAsync(operatorToCreate)
            );

            Assert.Equal($"User with id: {user.Id} is already operator to the post office with id: {postOffice.Id}. " +
              $"Try to remove operator from that post office and try again", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldAssignOperatorRoleToUser_WhenUserIsCreated()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var postOffice = await CreateTestPostOffice1Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), user.Id, postOffice.Id);
            Assert.True(operatorResult.IsSuccess);
            var operatorToCreate = operatorResult.Value;

            // Act
            await operatorsRepository.CreateAsync(operatorToCreate);

            // Assert
            var createdUser = await userManager.FindByIdAsync(user.Id.ToString());
            Assert.NotNull(createdUser);
            Assert.True(await userManager.IsInRoleAsync(createdUser, "Operator"));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var postOffice = await CreateTestPostOffice1Async();

            var operatorResult = Operator.Create(Guid.NewGuid(), Guid.NewGuid(), postOffice.Id);
            Assert.True(operatorResult.IsSuccess);
            var operatorToCreate = operatorResult.Value;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                async () => await operatorsRepository.CreateAsync(operatorToCreate)
            );

            Assert.Equal("User not found", exception.Message);
        }
    }
}
