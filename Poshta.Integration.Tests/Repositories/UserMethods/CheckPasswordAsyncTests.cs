namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class CheckPasswordAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task CheckPasswordAsync_WhenUserExistsAndPasswordIsCorrect_ReturnsTrue()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var correctPassword = "password"; // Предполагается, что этот пароль был установлен

            // Act
            var result = await usersRepository.CheckPasswordAsync(user.Id, correctPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckPasswordAsync_WhenUserExistsAndPasswordIsIncorrect_ReturnsFalse()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var incorrectPassword = "wrongpassword";

            // Act
            var result = await usersRepository.CheckPasswordAsync(user.Id, incorrectPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckPasswordAsync_WhenUserDoesNotExist_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                usersRepository.CheckPasswordAsync(Guid.NewGuid(), "password"));

            Assert.Contains("User with id:", exception.Message);
        }
    }
}
