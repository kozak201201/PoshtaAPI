using Poshta.Core.Exceptions;

namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class UpdateEmailAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task UpdateEmailAsync_WhenUserExists_UpdatesEmail()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var newEmail = "newemail@example.com";

            // Act
            await usersRepository.UpdateEmailAsync(user.Id, newEmail);
            var updatedUser = await usersRepository.GetByIdAsync(user.Id);

            // Assert
            Assert.Equal(newEmail, updatedUser?.Email);
        }

        [Fact]
        public async Task UpdateEmailAsync_WhenEmailAlreadyExists_ThrowsEmailAlreadyExistException()
        {
            // Arrange
            var user1 = await CreateTestUser1Async();
            var user2 = await CreateTestUser2Async();
            var existingEmail = user2.Email;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EmailAlreadyExistException>(() =>
                usersRepository.UpdateEmailAsync(user1.Id, existingEmail));

            Assert.Equal($"User with email: {existingEmail} already exists.", exception.Message);
        }

        [Fact]
        public async Task UpdateEmailAsync_WhenUserDoesNotExist_ThrowsException()
        {
            //Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                usersRepository.UpdateEmailAsync(userId, "test@example.com"));

            Assert.Equal("User with id: " + userId + " wasn't found", exception.Message);
        }
    }
}
