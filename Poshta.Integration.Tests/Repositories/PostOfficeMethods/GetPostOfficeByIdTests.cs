namespace Poshta.IntegrationTests.Repositories.PostOfficeMethods
{
    public class GetPostOfficeByIdAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetPostOfficeByIdAsync_ValidId_ReturnsPostOffice()
        {
            // Arrange
            var postOffice = await CreateTestPostOffice1Async();

            // Act
            var retrievedPostOffice = await postOfficesRepository.GetPostOfficeByIdAsync(postOffice.Id);

            // Assert
            Assert.NotNull(retrievedPostOffice);
            Assert.Equal(postOffice.Id, retrievedPostOffice.Id);
            Assert.Equal(postOffice.City, retrievedPostOffice.City);
            Assert.Equal(postOffice.Address, retrievedPostOffice.Address);
            Assert.Equal(postOffice.Number, retrievedPostOffice.Number);
        }

        [Fact]
        public async Task GetPostOfficeByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var retrievedPostOffice = await postOfficesRepository.GetPostOfficeByIdAsync(nonExistentId);

            // Assert
            Assert.Null(retrievedPostOffice);
        }
    }
}
