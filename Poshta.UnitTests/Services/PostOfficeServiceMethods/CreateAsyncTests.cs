using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.PostOfficeServiceMethods
{
    public class CreateAsyncTests : PostOfficeServiceTestsBase
    {
        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenPostOfficeIsValid()
        {
            // Arrange
            var postOfficeType = CreateDefaultPostOfficeType();
            var postOfficeTypeId = postOfficeType.Id;
            var number = 123;
            var city = "Test City";
            var address = "123 Test St";
            var maxShipmentsCount = 100;
            var latitude = 1.234;
            var longitude = 5.678;

            mockPostOfficeTypeService.Setup(x => x.GetByIdAsync(postOfficeTypeId))
                .ReturnsAsync(Result.Success(postOfficeType));

            mockPostOfficesRepository.Setup(x => x.CreateAsync(It.IsAny<PostOffice>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await postOfficeService.CreateAsync(number, city, address, maxShipmentsCount, latitude, longitude, postOfficeTypeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(number, result.Value.Number);
            Assert.Equal(city, result.Value.City);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_WhenPostOfficeTypeDoesNotExist()
        {
            // Arrange
            var postOfficeTypeId = Guid.NewGuid();
            var number = 123;
            var city = "Test City";
            var address = "123 Test St";
            var maxShipmentsCount = 100;
            var latitude = 1.234;
            var longitude = 5.678;

            mockPostOfficeTypeService.Setup(x => x.GetByIdAsync(postOfficeTypeId))
                .ReturnsAsync(Result.Failure<PostOfficeType>("Post office type not found"));

            // Act
            var result = await postOfficeService.CreateAsync(number, city, address, maxShipmentsCount, latitude, longitude, postOfficeTypeId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Post office type not found", result.Error);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_WhenPostOfficeCreationFails()
        {
            // Arrange
            var postOfficeType = CreateDefaultPostOfficeType();
            var postOfficeTypeId = postOfficeType.Id;
            var number = 123;
            var city = "Test City";
            var address = "123 Test St";
            var maxShipmentsCount = 100;
            var latitude = 1.234;
            var longitude = 5.678;

            mockPostOfficeTypeService.Setup(x => x.GetByIdAsync(postOfficeTypeId))
                .ReturnsAsync(Result.Success(postOfficeType));

            mockPostOfficesRepository.Setup(x => x.CreateAsync(It.IsAny<PostOffice>()))
                .Throws(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                postOfficeService.CreateAsync(number, city, address, maxShipmentsCount, latitude, longitude, postOfficeTypeId));

            Assert.Equal("Database error", exception.Message);
        }
    }
}
