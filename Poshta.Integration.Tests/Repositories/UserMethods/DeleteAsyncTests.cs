namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class DeleteAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task DeleteAsync_WhenUserExists_DeletesUser()
        {
            // Arrange
            var user = await CreateTestUser1Async(); // Создаем тестового пользователя

            // Act
            await usersRepository.DeleteAsync(user.Id);

            // Assert
            var deletedUser = await usersRepository.GetByIdAsync(user.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task DeleteAsync_WhenUserDoesNotExist_ThrowsException()
        {
            //Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                usersRepository.DeleteAsync(userId));

            Assert.Equal("User with id: " + userId + " wasn't found", exception.Message);
        }
    }
}
