using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class LoginAsyncTests : UserServiceTestsBase
    {
        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var phoneNumber = "+1234567890";
            var password = "hashedPassword";
            var userId = Guid.NewGuid();
            var user = User.Create(userId, "Doe", "John", password, phoneNumber).Value;

            usersRepository.Setup(repo => repo.GetByPhoneAsync(phoneNumber)).ReturnsAsync(user);
            usersRepository.Setup(repo => repo.CheckPasswordAsync(userId, password)).ReturnsAsync(true);
            usersRepository.Setup(repo => repo.GetRolesAsync(userId)).ReturnsAsync(new[] { "User" });

            var token = "mocked_token";
            jwtProvider.Setup(provider => provider.Generate(userId, It.IsAny<IList<string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Returns(token);

            // Act
            var result = await userService.LoginAsync(phoneNumber, password);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(token, result.Value);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var phoneNumber = "+1234567890";
            var password = "hashedPassword";

            usersRepository.Setup(repo => repo.GetByPhoneAsync(phoneNumber)).ReturnsAsync((User)null);

            // Act
            var result = await userService.LoginAsync(phoneNumber, password);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"user with phone: {phoneNumber} wasn't found", result.Error);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsFailure()
        {
            // Arrange
            var phoneNumber = "+1234567890";
            var password = "wrongPassword";
            var userId = Guid.NewGuid();
            var user = User.Create(userId, "Doe", "John", "hashedPassword", phoneNumber).Value;

            usersRepository.Setup(repo => repo.GetByPhoneAsync(phoneNumber)).ReturnsAsync(user);
            usersRepository.Setup(repo => repo.CheckPasswordAsync(userId, password)).ReturnsAsync(false);

            // Act
            var result = await userService.LoginAsync(phoneNumber, password);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid password", result.Error);
        }

        [Fact]
        public async Task LoginAsync_UserIsOperator_ReturnsTokenWithClaims()
        {
            // Arrange
            var phoneNumber = "+1234567890";
            var password = "hashedPassword";
            var userId = Guid.NewGuid();
            var userResult = User.Create(userId, "Doe", "John", password, phoneNumber);

            Assert.True(userResult.IsSuccess);

            var user = userResult.Value;

            usersRepository.Setup(repo => repo.GetByPhoneAsync(phoneNumber)).ReturnsAsync(user);
            usersRepository.Setup(repo => repo.CheckPasswordAsync(userId, password)).ReturnsAsync(true);
            usersRepository.Setup(repo => repo.GetRolesAsync(userId)).ReturnsAsync(new[] { "Operator" });

            var operatorId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();
            var operatorService = new Mock<IOperatorService>();

            var operatorPostOfficeResult = Operator.Create(operatorId, userId, postOfficeId);

            Assert.True(operatorPostOfficeResult.IsSuccess);

            var operatorPostOffice = operatorPostOfficeResult.Value;

            serviceProvider.Setup(sp => sp.GetService(typeof(IOperatorService))).Returns(operatorService.Object);

            operatorService.Setup(os => os.GetByUserIdAsync(userId)).ReturnsAsync(Result.Success(operatorPostOffice));

            var token = "mocked_token";
            jwtProvider.Setup(provider => provider.Generate(userId, It.IsAny<IList<string>>(), It.IsAny<Dictionary<string, string>>()))
                       .Returns(token);

            // Act
            var result = await userService.LoginAsync(phoneNumber, password);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(token, result.Value);
        }
    }
}
