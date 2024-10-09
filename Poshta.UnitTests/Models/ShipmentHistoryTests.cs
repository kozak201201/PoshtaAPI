using Poshta.Core.Models;

namespace Poshta.UnitTests.Models
{
    public class ShipmentHistoryTests
    {
        [Fact]
        public void Create_ValidShipmentHistory_ReturnsSuccessResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();
            var shipmentStatus = ShipmentStatus.InTransit;
            var statusDate = DateTime.UtcNow;
            var description = "Package is on its way.";

            // Act
            var result = ShipmentHistory.Create(id, shipmentId, shipmentStatus, postOfficeId, statusDate, description);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(id, result.Value.Id);
            Assert.Equal(shipmentId, result.Value.ShipmentId);
            Assert.Equal(shipmentStatus, result.Value.Status);
            Assert.Equal(postOfficeId, result.Value.PostOfficeId);
            Assert.Equal(statusDate, result.Value.StatusDate);
            Assert.Equal(description, result.Value.Description);
        }

        [Fact]
        public void Create_EmptyDescription_ReturnsFailureResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();
            var shipmentStatus = ShipmentStatus.InTransit;
            var statusDate = DateTime.UtcNow;
            var description = ""; // empty description

            // Act
            var result = ShipmentHistory.Create(id, shipmentId, shipmentStatus, postOfficeId, statusDate, description);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("description can't be null or empty", result.Error);
        }
    }
}
