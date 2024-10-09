using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.PostOfficeServiceMethods
{
    public class GetPostOfficeByIdTests : PostOfficeServiceTestsBase
    {
        [Fact]
        public async Task GetPostOfficeByIdAsync_ShouldReturnPostOffice_WhenFound()
        {
            // Arrange
            var postOfficeId = Guid.NewGuid();
            var postOfficeNumber = 1;
            var city = "Test City";
            var address = "123 Test St";
            var maxShipmentsCount = 20;
            var latitude = 45.0;
            var longitude = 30.0;

            var postOfficeType = CreateDefaultPostOfficeType();

            var postOfficeResult = PostOffice.Create(
                postOfficeId,
                postOfficeNumber,
                city,
                address,
                maxShipmentsCount,
                latitude,
                longitude,
                postOfficeType);

            Assert.True(postOfficeResult.IsSuccess);
            var expectedPostOffice = postOfficeResult.Value;

            mockPostOfficesRepository.Setup(x => x.GetPostOfficeByIdAsync(postOfficeId))
                .ReturnsAsync(expectedPostOffice);

            // Act
            var result = await postOfficeService.GetPostOfficeByIdAsync(postOfficeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedPostOffice, result.Value);
        }

        [Fact]
        public async Task GetPostOfficeByIdAsync_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            var postOfficeId = Guid.NewGuid();

            mockPostOfficesRepository.Setup(x => x.GetPostOfficeByIdAsync(postOfficeId))
                .ReturnsAsync((PostOffice)null);

            // Act
            var result = await postOfficeService.GetPostOfficeByIdAsync(postOfficeId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Post office with id: {postOfficeId} wasn't found", result.Error);
        }
    }
}

