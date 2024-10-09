using Microsoft.Extensions.Logging;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class AddRoleTests : UserServiceTestsBase
    {
        [Fact]
        public async Task AddRole_UserExists_AddsRoleSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = "Admin";

            usersRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+123456789").Value);

            // Act
            var result = await userService.AddRoleAsync(userId, role);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"role {role} add successfully to user: {userId}", result.Value);
            usersRepository.Verify(repo => repo.AddRoleAsync(userId, role), Times.Once);
        }

        [Fact]
        public async Task AddRole_UserDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = "Admin";

            usersRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await userService.AddRoleAsync(userId, role);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"User with id: {userId} wasn't found", result.Error);
        }
    }
}
