namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class GetRolesAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetRolesAsync_WhenUserExists_ReturnsRoles()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var roleName = "Admin";
            await usersRepository.AddRoleAsync(user.Id, roleName);

            // Act
            var roles = await usersRepository.GetRolesAsync(user.Id);

            // Assert
            Assert.Contains(roleName, roles);
        }

        [Fact]
        public async Task GetRolesAsync_WhenUserDoesNotExist_ThrowsException()
        {
            //Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                usersRepository.GetRolesAsync(userId));

            Assert.Equal("User with id: " + userId + " wasn't found", exception.Message);
        }
    }
}
