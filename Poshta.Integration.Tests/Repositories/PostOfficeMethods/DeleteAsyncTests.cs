namespace Poshta.IntegrationTests.Repositories.PostOfficeMethods
{
    public class DeleteAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task DeleteAsync_ExistingPostOffice_DeletesPostOffice()
        {
            // Arrange
            var postOffice = await CreateTestPostOffice1Async();

            // Act
            await postOfficesRepository.DeleteAsync(postOffice.Id);

            // Assert
            var deletedPostOffice = await postOfficesRepository.GetPostOfficeByIdAsync(postOffice.Id);
            Assert.Null(deletedPostOffice);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingPostOffice_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                postOfficesRepository.DeleteAsync(nonExistingId));

            Assert.Equal($"Post office with id: {nonExistingId} wasn't found", exception.Message);
        }
    }
}
