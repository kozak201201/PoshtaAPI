namespace Poshta.IntegrationTests.Repositories.PostOfficeMethods
{
    public class GetPostOfficesAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetPostOfficesAsync_WhenNoPostOffices_ReturnsEmptyList()
        {
            // Act
            var postOffices = await postOfficesRepository.GetPostOfficesAsync();

            // Assert
            Assert.Empty(postOffices);
        }

        [Fact]
        public async Task GetPostOfficesAsync_WhenPostOfficesExist_ReturnsListOfPostOffices()
        {
            // Arrange
            await CreateTestPostOffice1Async(); // Create first post office

            // Act
            var postOffices = await postOfficesRepository.GetPostOfficesAsync();

            // Assert
            Assert.NotEmpty(postOffices);
            Assert.Single(postOffices);
        }

        [Fact]
        public async Task GetPostOfficesAsync_WhenMultiplePostOfficesExist_ReturnsAllPostOffices()
        {
            // Arrange
            await CreateTestPostOffice1Async(); // Create first post office
            await CreateTestPostOffice2Async(); // Create second post office

            // Act
            var postOffices = await postOfficesRepository.GetPostOfficesAsync();

            // Assert
            Assert.NotEmpty(postOffices);
            Assert.Equal(2, postOffices.Count());
        }
    }
}
