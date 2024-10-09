using Poshta.Core.Models;

namespace Poshta.UnitTests.Models
{
    public class PostOfficeTypeTests
    {
        [Fact]
        public void Create_ValidPostOfficeType_ReturnsPostOfficeType()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Standard";
            var maxShipmentWeight = 30f;
            var maxShipmentLength = 100f;
            var maxShipmentWidth = 50f;
            var maxShipmentHeight = 50f;

            // Act
            var result = PostOfficeType.Create(id, name, maxShipmentWeight, maxShipmentLength, maxShipmentWidth, maxShipmentHeight);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(name, result.Value.Name);
            Assert.Equal(maxShipmentWeight, result.Value.MaxShipmentWeight);
        }

        [Fact]
        public void Create_EmptyName_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "";
            var maxShipmentWeight = 30f;
            var maxShipmentLength = 100f;
            var maxShipmentWidth = 50f;
            var maxShipmentHeight = 50f;

            // Act
            var result = PostOfficeType.Create(id, name, maxShipmentWeight, maxShipmentLength, maxShipmentWidth, maxShipmentHeight);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Name can't be null or empty string", result.Error);
        }

        [Fact]
        public void Create_NegativeWeight_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Standard";
            var maxShipmentWeight = -1f;
            var maxShipmentLength = 100f;
            var maxShipmentWidth = 50f;
            var maxShipmentHeight = 50f;

            // Act
            var result = PostOfficeType.Create(id, name, maxShipmentWeight, maxShipmentLength, maxShipmentWidth, maxShipmentHeight);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Weight can't be equel or less than 0", result.Error);
        }

        [Fact]
        public void Create_NegativeLength_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Standard";
            var maxShipmentWeight = 30f;
            var maxShipmentLength = -1f;
            var maxShipmentWidth = 50f;
            var maxShipmentHeight = 50f;

            // Act
            var result = PostOfficeType.Create(id, name, maxShipmentWeight, maxShipmentLength, maxShipmentWidth, maxShipmentHeight);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Length can't be equel or less than 0", result.Error);
        }

        [Fact]
        public void Create_NegativeWidth_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Standard";
            var maxShipmentWeight = 30f;
            var maxShipmentLength = 100f;
            var maxShipmentWidth = -1f;
            var maxShipmentHeight = 50f;

            // Act
            var result = PostOfficeType.Create(id, name, maxShipmentWeight, maxShipmentLength, maxShipmentWidth, maxShipmentHeight);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Width can't be equel or less than 0", result.Error);
        }

        [Fact]
        public void Create_NegativeHeight_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Standard";
            var maxShipmentWeight = 30f;
            var maxShipmentLength = 100f;
            var maxShipmentWidth = 50f;
            var maxShipmentHeight = -1f;

            // Act
            var result = PostOfficeType.Create(id, name, maxShipmentWeight, maxShipmentLength, maxShipmentWidth, maxShipmentHeight);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Height can't be equel or less than 0", result.Error);
        }
    }
}
