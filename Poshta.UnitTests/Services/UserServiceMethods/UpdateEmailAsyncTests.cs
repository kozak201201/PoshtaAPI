using Microsoft.Extensions.Logging;
using Moq;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class UpdateEmailAsyncTests : UserServiceTestsBase
    {
        [Fact]
        public async Task UpdateEmailAsync_ValidCode_UpdatesEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var newEmail = "test@example.com";
            var confirmationCode = "123456";

            confirmationCodeService.Setup(service => service.ValidateCodeAsync(newEmail, confirmationCode))
                .ReturnsAsync(true);

            // Act
            var result = await userService.UpdateEmailAsync(userId, newEmail, confirmationCode);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"Success update email: {newEmail} for user with id: {userId}", result.Value);
            usersRepository.Verify(repo => repo.UpdateEmailAsync(userId, newEmail), Times.Once);
        }

        [Fact]
        public async Task UpdateEmailAsync_InvalidCode_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var newEmail = "test@example.com";
            var confirmationCode = "invalid";

            confirmationCodeService.Setup(service => service.ValidateCodeAsync(newEmail, confirmationCode))
                .ReturnsAsync(false);

            // Act
            var result = await userService.UpdateEmailAsync(userId, newEmail, confirmationCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid or expired code", result.Error);
        }
    }
}
