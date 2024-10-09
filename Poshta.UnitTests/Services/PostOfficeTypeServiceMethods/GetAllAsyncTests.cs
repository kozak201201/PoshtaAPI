using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.PostOfficeTypeServiceMethods
{
    public class GetAllAsyncTests : PostOfficeTypeServiceTestsBase
    {
        [Fact]
        public async Task GetAllAsync_ReturnsPostOfficeTypes()
        {
            // Arrange
            var postOfficeType1Result = PostOfficeType.Create(Guid.NewGuid(), "Standard", 10f, 50f, 30f, 20f);
            var postOfficeType2Result = PostOfficeType.Create(Guid.NewGuid(), "Express", 5f, 25f, 15f, 10f);

            Assert.True(postOfficeType1Result.IsSuccess && postOfficeType2Result.IsSuccess);

            var postOfficeType1 = postOfficeType1Result.Value;
            var postOfficeType2 = postOfficeType2Result.Value;

            var postOfficeTypes = new List<PostOfficeType>
            {
                postOfficeType1,
                postOfficeType2
            };

            postOfficeTypesRepository
                .Setup(repo => repo.GetPostOfficeTypesAsync())
                .ReturnsAsync(postOfficeTypes);

            // Act
            var result = await postOfficeTypeService.GetAllAsync();

            // Assert
            Assert.Equal(postOfficeTypes, result);
        }

        [Fact]
        public async Task GetAllAsync_NoPostOfficeTypes_ReturnsEmptyList()
        {
            // Arrange
            postOfficeTypesRepository
                .Setup(repo => repo.GetPostOfficeTypesAsync())
                .ReturnsAsync(new List<PostOfficeType>());

            // Act
            var result = await postOfficeTypeService.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}
