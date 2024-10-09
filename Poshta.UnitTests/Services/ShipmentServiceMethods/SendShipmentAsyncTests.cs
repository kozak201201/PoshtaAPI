using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.ShipmentServiceMethods
{
    public class SendShipmentAsyncTests : ShipmentServiceTestsBase
    {
        [Fact]
        public async Task SendShipmentAsync_ValidParameters_SendsShipment()
        {
            // Arrange
            var shipmentId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();
            var operatorPostOfficeId = postOfficeId;

            var postOfficeTypeResult = PostOfficeType.Create(
                Guid.NewGuid(),
                "Type",
                10f,
                10f,
                10f,
                10f
            ).Value;

            var postOfficeResult = PostOffice.Create(
                postOfficeId,
                1,
                "City",
                "Address",
                100,
                50.0,
                30.0,
                postOfficeTypeResult
            ).Value;

            postOfficeServiceMock
                .Setup(x => x.GetPostOfficeByIdAsync(postOfficeId))
                .ReturnsAsync(Result.Success(postOfficeResult));

            var shipmentResult = Shipment.Create(
                shipmentId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                postOfficeId,
                Guid.NewGuid(),
                PayerType.Sender,
                "12345678901234",
                100.0,
                100.0,
                1.0f,
                20.0f,
                15.0f,
                10.0f
            );

            Assert.True(shipmentResult.IsSuccess);
            var shipment = shipmentResult.Value;

            shipment.CurrentPostOfficeId = postOfficeId;
            shipment.Status = ShipmentStatus.AtPostOffice;

            shipmentsRepositoryMock
                .Setup(x => x.GetByIdAsync(shipmentId))
                .ReturnsAsync(shipment);

            // Act
            var result = await shipmentService.SendShipmentAsync(shipmentId, operatorPostOfficeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("left from", result.Value);
            shipmentsRepositoryMock.Verify(x => x.UpdateStatusAsync(shipmentId, ShipmentStatus.InTransit), Times.Once);
            shipmentsRepositoryMock.Verify(x => x.UpdateCurrentPostOfficeAsync(shipmentId, null), Times.Once);
        }

        [Fact]
        public async Task SendShipmentAsync_ShipmentNotFound_ReturnsFailure()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();

            shipmentsRepositoryMock
                .Setup(x => x.GetByIdAsync(shipmentId))
                .ReturnsAsync((Shipment)null); // Shipment not found

            // Act
            var result = await shipmentService.SendShipmentAsync(shipmentId, operatorId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Shipment with id: {shipmentId} wasn't found", result.Error);
        }

        [Fact]
        public async Task SendShipmentAsync_NoCurrentPostOffice_ReturnsFailure()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();

            var shipmentResult = Shipment.Create(
                shipmentId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                PayerType.Sender,
                "12345678901234",
                100.0,
                100.0,
                1.0f,
                20.0f,
                15.0f,
                10.0f
            );

            Assert.True(shipmentResult.IsSuccess);
            var shipment = shipmentResult.Value;

            shipmentsRepositoryMock
                .Setup(x => x.GetByIdAsync(shipmentId))
                .ReturnsAsync(shipment);

            // Act
            var result = await shipmentService.SendShipmentAsync(shipmentId, operatorId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Shipment doesn't have current post office", result.Error);
        }

        [Fact]
        public async Task SendShipmentAsync_OperatorDoesNotMatchPostOffice_ReturnsFailure()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();
            var operatorPostOfficeId = Guid.NewGuid(); // Different post office

            var shipmentResult = Shipment.Create(
                shipmentId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                postOfficeId,
                Guid.NewGuid(),
                PayerType.Sender,
                "12345678901234",
                100.0,
                100.0,
                1.0f,
                20.0f,
                15.0f,
                10.0f
            );

            Assert.True(shipmentResult.IsSuccess);
            var shipment = shipmentResult.Value;

            shipment.CurrentPostOfficeId = postOfficeId;
            shipmentsRepositoryMock
                .Setup(x => x.GetByIdAsync(shipmentId))
                .ReturnsAsync(shipment);

            // Act
            var result = await shipmentService.SendShipmentAsync(shipmentId, operatorId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("doesn't work at this post office", result.Error);
        }
    }
}
