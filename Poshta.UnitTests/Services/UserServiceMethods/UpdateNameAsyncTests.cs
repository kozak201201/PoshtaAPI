using Moq;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class UpdateNameAsyncTests : UserServiceTestsBase
    {
        [Fact]
        public async Task UpdateNameAsync_ValidInput_ReturnsSuccessResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            string? middleName = "TestMiddleName";

            // Act
            var result = await userService.UpdateNameAsync(userId, firstName, lastName, middleName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"Success update user name. User id: {userId}", result.Value);
            usersRepository.Verify(repo => repo.UpdateNameAsync(userId, firstName, lastName, middleName), Times.Once);
        }

        [Fact]
        public async Task UpdateNameAsync_InvalidLastName_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var firstName = "TestFirstName";
            var lastName = "Invalid@Name";
            string? middleName = "TestMiddleName";

            // Act
            var result = await userService.UpdateNameAsync(userId, firstName, lastName, middleName);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid last name", result.Error);
            usersRepository.Verify(repo => repo.UpdateNameAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateNameAsync_InvalidFirstName_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var firstName = "Invalid@Name";
            var lastName = "TestLastName";
            string? middleName = "TestMiddleName";

            // Act
            var result = await userService.UpdateNameAsync(userId, firstName, lastName, middleName);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid first name", result.Error);
            usersRepository.Verify(repo => repo.UpdateNameAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateNameAsync_InvalidMiddleName_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            string? middleName = "Invalid@Name";

            // Act
            var result = await userService.UpdateNameAsync(userId, firstName, lastName, middleName);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Invalid middle name", result.Error);
            usersRepository.Verify(repo => repo.UpdateNameAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
