namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class UpdateNameAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task UpdateNameAsync_WhenUserExists_UpdatesName()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var newFirstName = "NewFirstName";
            var newLastName = "NewLastName";
            var newMiddleName = "NewMiddleName";

            // Act
            await usersRepository.UpdateNameAsync(user.Id, newFirstName, newLastName, newMiddleName);

            // Assert
            var updatedUser = await usersRepository.GetByIdAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(newFirstName, updatedUser.FirstName);
            Assert.Equal(newLastName, updatedUser.LastName);
            Assert.Equal(newMiddleName, updatedUser.MiddleName);
        }

        [Fact]
        public async Task UpdateNameAsync_WhenUserDoesNotExist_ThrowsException()
        {
            //Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                usersRepository.UpdateNameAsync(userId, "FirstName", "LastName", "MiddleName"));

            Assert.Equal("User with id: " + userId + " wasn't found", exception.Message);
        }
    }
}
