using Poshta.Core.Exceptions;
using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class CreateAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task CreateAsync_ValidUser_CreatesUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = User.Create(userId, "LastName", "FirstName", "password", "+1234567890").Value;

            // Act
            await usersRepository.CreateAsync(user);

            // Assert
            var createdUser = await usersRepository.GetByIdAsync(userId);
            Assert.NotNull(createdUser);
            Assert.Equal(userId, createdUser.Id);
            Assert.Equal(user.PhoneNumber, createdUser.PhoneNumber);
        }

        [Fact]
        public async Task CreateAsync_DuplicatePhoneNumber_ThrowsPhoneAlreadyExistException()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var user1 = User.Create(userId1, "TestLastName", "TestFirstName", "password", "+1234567890").Value;
            await usersRepository.CreateAsync(user1);

            var userId2 = Guid.NewGuid();
            var user2 = User.Create(userId2, "TestLastName", "TestFirstName", "password", "+1234567890").Value;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PhoneAlreadyExistException>(() =>
                usersRepository.CreateAsync(user2));

            Assert.Equal($"User with phone: +1234567890 already exists.", exception.Message);
        }
    }
}
