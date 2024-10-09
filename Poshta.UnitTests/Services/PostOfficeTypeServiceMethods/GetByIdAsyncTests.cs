using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.PostOfficeTypeServiceMethods
{
    public class GetByIdAsyncTests : PostOfficeTypeServiceTestsBase
    {
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsPostOfficeType()
        {
            // Arrange
            var postOfficeTypeId = Guid.NewGuid();
            var postOfficeTypeResult = PostOfficeType.Create(postOfficeTypeId, "Standard", 10f, 50f, 30f, 20f);

            Assert.True(postOfficeTypeResult.IsSuccess);

            var postOfficeType = postOfficeTypeResult.Value;

            postOfficeTypesRepository
                .Setup(repo => repo.GetPostOfficeTypeByIdAsync(postOfficeTypeId))
                .ReturnsAsync(postOfficeType);

            // Act
            var result = await postOfficeTypeService.GetByIdAsync(postOfficeTypeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(postOfficeType, result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsFailureResult()
        {
            // Arrange
            var postOfficeTypeId = Guid.NewGuid();

            postOfficeTypesRepository
                .Setup(repo => repo.GetPostOfficeTypeByIdAsync(postOfficeTypeId))
                .ReturnsAsync((PostOfficeType)null);

            // Act
            var result = await postOfficeTypeService.GetByIdAsync(postOfficeTypeId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Post office type with id: {postOfficeTypeId} wasn't found", result.Error);
        }
    }
}
