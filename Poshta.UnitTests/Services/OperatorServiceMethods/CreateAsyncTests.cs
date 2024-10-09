using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.OperatorServiceMethods
{
    public class CreateAsyncTests : OperatorServiceTestsBase
    {
        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();
            var userResult = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+1234567890");

            Assert.True(userResult.IsSuccess);

            var user = userResult.Value;

            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(Result.Success(user));
            mockOperatorsRepository.Setup(x => x.CreateAsync(It.IsAny<Operator>())).Returns(Task.CompletedTask);

            // Act
            var result = await operatorService.CreateAsync(userId, postOfficeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            mockOperatorsRepository.Verify(x => x.CreateAsync(It.IsAny<Operator>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(Result.Failure<User>("User not found"));

            // Act
            var result = await operatorService.CreateAsync(userId, postOfficeId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found", result.Error);
            mockOperatorsRepository.Verify(x => x.CreateAsync(It.IsAny<Operator>()), Times.Never);
        }
    }
}
