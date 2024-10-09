using Microsoft.Extensions.Logging;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class GetUsersAsyncTests : UserServiceTestsBase
    {
        [Fact]
        public async Task GetUsersAsync_ReturnsUsers()
        {
            // Arrange
            var usersList = new List<User>
            {
                User.Create(Guid.NewGuid(), "TestLastName", "TestFirstName", "hashedPassword1", "+1234567890").Value,
                User.Create(Guid.NewGuid(), "TestLastName", "TestFirstName", "hashedPassword2", "+1234567891").Value
            };
            usersRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(usersList);

            // Act
            var result = await userService.GetUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usersList.Count, result.Count());
        }
    }
}
