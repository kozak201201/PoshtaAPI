using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class GetByIdAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetByIdAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var user = await CreateTestUser1Async(); // Создаем тестового пользователя

            // Act
            var retrievedUser = await usersRepository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id, retrievedUser.Id);
            Assert.Equal(user.PhoneNumber, retrievedUser.PhoneNumber);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var retrievedUser = await usersRepository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(retrievedUser);
        }
    }
}
