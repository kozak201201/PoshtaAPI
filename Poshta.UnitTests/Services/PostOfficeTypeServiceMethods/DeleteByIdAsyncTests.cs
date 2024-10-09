using Moq;

namespace Poshta.UnitTests.Services.PostOfficeTypeServiceMethods
{
    public class DeleteByIdAsyncTests : PostOfficeTypeServiceTestsBase
    {
        [Fact]
        public async Task DeleteByIdAsync_ValidId_ReturnsSuccessMessage()
        {
            // Arrange
            var postOfficeTypeId = Guid.NewGuid();

            // Act
            var result = await postOfficeTypeService.DeleteByIdAsync(postOfficeTypeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"Success delete post office type with id: {postOfficeTypeId}", result.Value);
        }

        [Fact]
        public async Task DeleteByIdAsync_InvalidId_CallsDeleteOnce()
        {
            // Arrange
            var postOfficeTypeId = Guid.NewGuid();

            postOfficeTypesRepository
                .Setup(repo => repo.DeleteAsync(postOfficeTypeId))
                .ThrowsAsync(new Exception("Deletion failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                postOfficeTypeService.DeleteByIdAsync(postOfficeTypeId));
        }
    }
}
