namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class GetByEmailAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var user = await CreateTestUser1Async();

            // Act
            var foundUser = await usersRepository.GetByEmailAsync(user.Email);

            // Assert
            Assert.NotNull(foundUser);
            Assert.Equal(user.Id, foundUser.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Act
            var foundUser = await usersRepository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(foundUser);
        }
    }
}
