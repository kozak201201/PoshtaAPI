using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Models;
using Twilio.Types;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class RegistrationAsyncTests : UserServiceTestsBase
    {
        [Fact]
        public async Task RegisterAsync_ValidData_ReturnsSuccessResult()
        {
            // Arrange
            var phoneNumber = "1234567890";
            var confirmationCode = "1234";
            var userId = Guid.NewGuid();

            confirmationCodeService.Setup(x => x.ValidateCodeAsync(phoneNumber, confirmationCode)).ReturnsAsync(true);
            usersRepository.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(Result.Success());

            // Act
            var result = await userService.RegisterAsync("TestLastName", "TestFirstName", "password", phoneNumber, confirmationCode);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("Success registration user. User id:", result.Value);
        }

        [Fact]
        public async Task RegisterAsync_InvalidCode_ReturnsFailureResult()
        {
            // Arrange
            var phoneNumber = "1234567890";
            var confirmationCode = "1234";

            confirmationCodeService.Setup(x => x.ValidateCodeAsync(phoneNumber, confirmationCode)).ReturnsAsync(false);

            // Act
            var result = await userService.RegisterAsync("TestLastName", "TestFirstName", "password", phoneNumber, confirmationCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid or expired code", result.Error);
        }

        [Fact]
        public async Task RegisterAsync_UserCreationFails_ReturnsFailureResult()
        {
            // Arrange
            var lastName = "Doe";
            var firstName = "John";
            var password = "password";
            var phoneNumber = "+1234567890";
            var confirmationCode = "123456";

            confirmationCodeService.Setup(service => service.ValidateCodeAsync(phoneNumber, confirmationCode))
                .ReturnsAsync(true);

            // Настраиваем Mock для создания пользователя, который вызывает ошибку
            usersRepository.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("User creation failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                userService.RegisterAsync(lastName, firstName, password, phoneNumber, confirmationCode));

            Assert.Equal("User creation failed", exception.Message);
        }


        [Fact]
        public async Task RegisterAsync_EmptyFirstName_ReturnsFailureResult()
        {
            // Arrange
            var phoneNumber = "1234567890";
            var confirmationCode = "1234";

            confirmationCodeService.Setup(x => x.ValidateCodeAsync(phoneNumber, confirmationCode)).ReturnsAsync(true);

            // Act
            var result = await userService.RegisterAsync("TestLastName", "", "password", phoneNumber, confirmationCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid first name", result.Error);
        }

        [Fact]
        public async Task RegisterAsync_EmptyPhoneNumber_ReturnsFailureResult()
        {
            // Arrange
            var confirmationCode = "1234";

            confirmationCodeService.Setup(x => x.ValidateCodeAsync("", confirmationCode)).ReturnsAsync(true);

            // Act
            var result = await userService.RegisterAsync("TestLastName", "TestFirstName", "password", "", confirmationCode);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid phone", result.Error);
        }
    }
}
