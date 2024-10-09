using Microsoft.Extensions.Logging;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class GetUserByPhoneAsyncTests : UserServiceTestsBase
    {
        [Fact]
        public async Task GetUserByPhoneAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var phone = "+1234567890";
            var userId = Guid.NewGuid();
            var existingUser = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", phone).Value;
            usersRepository.Setup(repo => repo.GetByPhoneAsync(phone)).ReturnsAsync(existingUser);

            // Act
            var result = await userService.GetUserByPhoneAsync(phone);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(existingUser, result.Value);
        }

        [Fact]
        public async Task GetUserByPhoneAsync_UserDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            var phone = "+1234567890";
            usersRepository.Setup(repo => repo.GetByPhoneAsync(phone)).ReturnsAsync((User)null);

            // Act
            var result = await userService.GetUserByPhoneAsync(phone);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("User with this phone wasn't found", result.Error);
        }
    }
}
