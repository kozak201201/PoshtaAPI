using Poshta.Core.Models;

namespace Poshta.UnitTests.Models
{
    public class PostOfficeTests
    {
        [Fact]
        public void Create_ValidPostOffice_ReturnsPostOffice()
        {
            // Arrange
            var id = Guid.NewGuid();
            var number = 1;
            var city = "TestCity";
            var address = "123 Test St";
            var maxShipmentsCount = 20;
            var latitude = 45.0;
            var longitude = 30.0;

            var postOfficeTypeId = Guid.NewGuid();
            var postOfficeType = PostOfficeType.Create(postOfficeTypeId, "Standard", 30, 100, 50, 50).Value;

            // Act
            var result = PostOffice.Create(id, number, city, address, maxShipmentsCount, latitude, longitude, postOfficeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(city, result.Value.City);
            Assert.Equal(address, result.Value.Address);
            Assert.Equal(number, result.Value.Number);
        }

        [Fact]
        public void Create_InvalidNumber_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var number = 0;
            var city = "TestCity";
            var address = "123 Test St";
            var maxShipmentsCount = 20;
            var latitude = 45.0;
            var longitude = 30.0;

            var postOfficeTypeId = Guid.NewGuid();
            var postOfficeType = PostOfficeType.Create(postOfficeTypeId, "Standard", 30, 100, 50, 50).Value;

            // Act
            var result = PostOffice.Create(id, number, city, address, maxShipmentsCount, latitude, longitude, postOfficeType);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("post office number cannot be less or equel 0", result.Error);
        }

        [Fact]
        public void Create_EmptyCity_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var number = 1;
            var city = "";
            var address = "123 Test St";
            var maxShipmentsCount = 20;
            var latitude = 45.0;
            var longitude = 30.0;

            var postOfficeTypeId = Guid.NewGuid();
            var postOfficeType = PostOfficeType.Create(postOfficeTypeId, "Standard", 30, 100, 50, 50).Value;

            // Act
            var result = PostOffice.Create(id, number, city, address, maxShipmentsCount, latitude, longitude, postOfficeType);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("city cannot be null or empty", result.Error);
        }

        [Fact]
        public void Create_EmptyAddress_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var number = 1;
            var city = "TestCity";
            var address = "";
            var maxShipmentsCount = 20;
            var latitude = 45.0;
            var longitude = 30.0;

            var postOfficeTypeId = Guid.NewGuid();
            var postOfficeType = PostOfficeType.Create(postOfficeTypeId, "Standard", 30, 100, 50, 50).Value;

            // Act
            var result = PostOffice.Create(id, number, city, address, maxShipmentsCount, latitude, longitude, postOfficeType);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("address cannot be null or empty", result.Error);
        }

        [Fact]
        public void Create_InvalidMaxShipmentsCount_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var number = 1;
            var city = "TestCity";
            var address = "123 Test St";
            var maxShipmentsCount = 10; // Less than MIN_SHIPMENTS_COUNT
            var latitude = 45.0;
            var longitude = 30.0;

            var postOfficeTypeId = Guid.NewGuid();
            var postOfficeType = PostOfficeType.Create(postOfficeTypeId, "Standard", 30, 100, 50, 50).Value;

            // Act
            var result = PostOffice.Create(id, number, city, address, maxShipmentsCount, latitude, longitude, postOfficeType);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("max shipments count cannot be less then 15", result.Error);
        }

        [Fact]
        public void Create_InvalidLatitude_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var number = 1;
            var city = "TestCity";
            var address = "123 Test St";
            var maxShipmentsCount = 20;
            var latitude = 100.0; // Invalid latitude
            var longitude = 30.0;

            var postOfficeTypeId = Guid.NewGuid();
            var postOfficeType = PostOfficeType.Create(postOfficeTypeId, "Standard", 30, 100, 50, 50).Value;

            // Act
            var result = PostOffice.Create(id, number, city, address, maxShipmentsCount, latitude, longitude, postOfficeType);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("latitude cannot be less then -90 or more than 90", result.Error);
        }

        [Fact]
        public void Create_InvalidLongitude_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var number = 1;
            var city = "TestCity";
            var address = "123 Test St";
            var maxShipmentsCount = 20;
            var latitude = 45.0;
            var longitude = 200.0; // Invalid longitude

            var postOfficeTypeId = Guid.NewGuid();
            var postOfficeType = PostOfficeType.Create(postOfficeTypeId, "Standard", 30, 100, 50, 50).Value;

            // Act
            var result = PostOffice.Create(id, number, city, address, maxShipmentsCount, latitude, longitude, postOfficeType);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("longitude cannot be less then -180 or more than 180", result.Error);
        }
    }
}
