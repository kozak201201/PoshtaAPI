using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class GetUserByIdAsyncTests : UserServiceTestsBase
    {
        [Fact]
        public async Task GetUserByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+1234567890").Value;
            usersRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existingUser);

            // Act
            var result = await userService.GetUserByIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(existingUser, result.Value);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            usersRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await userService.GetUserByIdAsync(userId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("User with this phone wasn't found", result.Error);
        }
    }
}
