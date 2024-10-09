using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class RemoveRoleAsyncTests : UserServiceTestsBase
    {
        [Fact]
        public async Task RemoveRole_UserExists_RemovesRoleSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = "Admin";

            usersRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+123456789").Value);
            usersRepository.Setup(repo => repo.GetRolesAsync(userId))
                .ReturnsAsync(new List<string> { role });

            // Act
            var result = await userService.RemoveRoleAsync(userId, role);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"Role {role} removed successfully from user: {userId}", result.Value);
            usersRepository.Verify(repo => repo.RemoveRoleAsync(userId, role), Times.Once);
        }

        [Fact]
        public async Task RemoveRole_UserDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = "Admin";

            usersRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await userService.RemoveRoleAsync(userId, role);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"User with id: {userId} wasn't found", result.Error);
        }
    }

}
