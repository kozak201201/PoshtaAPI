using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class GetByPhoneAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetByPhoneAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var user = await CreateTestUser1Async();

            // Act
            var foundUser = await usersRepository.GetByPhoneAsync(user.PhoneNumber);

            // Assert
            Assert.NotNull(foundUser);
            Assert.Equal(user.Id, foundUser.Id);
        }

        [Fact]
        public async Task GetByPhoneAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Act
            var foundUser = await usersRepository.GetByPhoneAsync("nonexistent_phone");

            // Assert
            Assert.Null(foundUser);
        }
    }
}
