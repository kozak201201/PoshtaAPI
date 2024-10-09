using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.PostOfficeTypeMethods
{
    public class GetPostOfficeTypeByIdAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetPostOfficeTypeByIdAsync_ExistingId_ReturnsPostOfficeType()
        {
            // Arrange
            var id = Guid.NewGuid();
            var postOfficeTypeResult = PostOfficeType.Create(id, "PostOfficeUpTo30Kg", 30f, 100f, 50f, 50f);

            Assert.True(postOfficeTypeResult.IsSuccess);
            var postOfficeType = postOfficeTypeResult.Value;

            await postOfficeTypesRepository.CreateAsync(postOfficeType);

            // Act
            var result = await postOfficeTypesRepository.GetPostOfficeTypeByIdAsync(postOfficeType.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(postOfficeType.Id, result?.Id);
        }

        [Fact]
        public async Task GetPostOfficeTypeByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var result = await postOfficeTypesRepository.GetPostOfficeTypeByIdAsync(nonExistingId);

            // Assert
            Assert.Null(result);
        }
    }
}
