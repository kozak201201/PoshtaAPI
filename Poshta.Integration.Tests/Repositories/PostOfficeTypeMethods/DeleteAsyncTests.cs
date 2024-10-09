using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.PostOfficeTypeMethods
{
    public class DeleteAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task DeleteAsync_ExistingId_DeletesPostOfficeType()
        {
            // Arrange
            var id = Guid.NewGuid();
            var postOfficeTypeResult = PostOfficeType.Create(id, "PostOfficeUpTo30Kg", 30f, 100f, 50f, 50f);

            Assert.True(postOfficeTypeResult.IsSuccess);
            var postOfficeType = postOfficeTypeResult.Value;

            await postOfficeTypesRepository.CreateAsync(postOfficeType);

            // Act
            await postOfficeTypesRepository.DeleteAsync(postOfficeType.Id);

            // Assert
            var deletedPostOfficeType = await context.PostOfficeTypes.FindAsync(postOfficeType.Id);
            Assert.Null(deletedPostOfficeType);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ThrowsException()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                postOfficeTypesRepository.DeleteAsync(nonExistingId));

            Assert.Equal($"Post office with id: {nonExistingId} wasn't found", exception.Message);
        }
    }
}
