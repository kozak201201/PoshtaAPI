using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.PostOfficeTypeServiceMethods
{
    public class CreateAsyncTests : PostOfficeTypeServiceTestsBase
    {
        [Fact]
        public async Task CreateAsync_ValidData_CreatesPostOfficeTypeSuccessfully()
        {
            // Arrange
            var name = "Standard";
            float maxShipmentWeight = 10f;
            float maxShipmentLength = 50f;
            float maxShipmentWidth = 30f;
            float maxShipmentHeight = 20f;

            // Act
            var result = await postOfficeTypeService.CreateAsync(name, maxShipmentWeight, maxShipmentLength, maxShipmentWidth, maxShipmentHeight);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            postOfficeTypesRepository.Verify(repo => repo.CreateAsync(It.IsAny<PostOfficeType>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_InvalidData_ReturnsFailureResult()
        {
            // Arrange
            var name = ""; // Invalid name
            float maxShipmentWeight = 10f;
            float maxShipmentLength = 50f;
            float maxShipmentWidth = 30f;
            float maxShipmentHeight = 20f;

            // Act
            var result = await postOfficeTypeService.CreateAsync(name, maxShipmentWeight, maxShipmentLength, maxShipmentWidth, maxShipmentHeight);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Name can't be null or empty string", result.Error);
            postOfficeTypesRepository.Verify(repo => repo.CreateAsync(It.IsAny<PostOfficeType>()), Times.Never);
        }
    }
}
