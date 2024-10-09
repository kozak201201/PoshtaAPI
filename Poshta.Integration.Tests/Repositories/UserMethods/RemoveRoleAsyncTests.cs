namespace Poshta.IntegrationTests.Repositories.UserMethods
{
    public class RemoveRoleAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task RemoveRoleAsync_WhenUserExists_RemovesRoleSuccessfully()
        {
            // Arrange
            var user = await CreateTestUser1Async();
            var role = "Admin";
            await usersRepository.AddRoleAsync(user.Id, role);

            // Act
            await usersRepository.RemoveRoleAsync(user.Id, role);
            var roles = await usersRepository.GetRolesAsync(user.Id);

            // Assert
            Assert.DoesNotContain(role, roles);
        }

        [Fact]
        public async Task RemoveRoleAsync_WhenUserDoesNotExist_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                usersRepository.RemoveRoleAsync(Guid.NewGuid(), "Admin"));

            Assert.Contains("User with id:", exception.Message);
        }
    }

}
