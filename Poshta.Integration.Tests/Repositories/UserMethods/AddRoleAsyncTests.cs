namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class AddRoleAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task AddRoleAsync_WhenUserExists_AddsRoleSuccessfully()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var role = "Admin";

            // Act
            await usersRepository.AddRoleAsync(user.Id, role);
            var roles = await usersRepository.GetRolesAsync(user.Id);

            // Assert
            Assert.Contains(role, roles);
        }

        [Fact]
        public async Task AddRoleAsync_WhenUserDoesNotExist_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                usersRepository.AddRoleAsync(Guid.NewGuid(), "Admin"));

            Assert.Contains("User with id:", exception.Message);
        }
    }
}
