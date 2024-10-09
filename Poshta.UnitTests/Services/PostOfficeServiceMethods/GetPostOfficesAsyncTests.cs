using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.PostOfficeServiceMethods
{
    public class GetPostOfficesTests : PostOfficeServiceTestsBase
    {
        [Fact]
        public async Task GetPostOfficesAsync_ShouldReturnPostOffices_WhenFound()
        {
            // Arrange
            var postOfficeType = CreateDefaultPostOfficeType();

            var postOffice1Result = PostOffice.Create(
                Guid.NewGuid(),
                1,
                "City1",
                "Address1",
                20,
                45.0,
                30.0,
                postOfficeType);

            var postOffice2Result = PostOffice.Create(
                Guid.NewGuid(),
                2,
                "City2",
                "Address2",
                20,
                45.0,
                30.0,
                postOfficeType);

            Assert.True(postOffice1Result.IsSuccess && postOffice2Result.IsSuccess);

            var postOffice1 = postOffice1Result.Value;
            var postOffice2 = postOffice2Result.Value;

            var postOfficesList = new List<PostOffice> { postOffice1, postOffice2 };

            mockPostOfficesRepository.Setup(x => x.GetPostOfficesAsync())
                .ReturnsAsync(postOfficesList);

            // Act
            var result = await postOfficeService.GetPostOfficesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, po => po.Id == postOffice1.Id);
            Assert.Contains(result, po => po.Id == postOffice2.Id);
        }

        [Fact]
        public async Task GetPostOfficesAsync_ShouldReturnEmptyList_WhenNoPostOfficesFound()
        {
            // Arrange
            var emptyList = new List<PostOffice>();

            mockPostOfficesRepository.Setup(x => x.GetPostOfficesAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await postOfficeService.GetPostOfficesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
