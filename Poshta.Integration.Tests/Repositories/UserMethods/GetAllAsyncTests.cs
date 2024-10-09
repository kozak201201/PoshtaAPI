namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class GetAllAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetAllAsync_WhenUsersExist_ReturnsAllUsers()
        {
            // Arrange
            await CreateTestUser1Async();
            await CreateTestUser2Async();

            // Act
            var users = await usersRepository.GetAllAsync();

            // Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenNoUsersExist_ReturnsEmptyList()
        {
            // Act
            var users = await usersRepository.GetAllAsync();

            // Assert
            Assert.NotNull(users);
            Assert.Empty(users);
        }
    }
}
