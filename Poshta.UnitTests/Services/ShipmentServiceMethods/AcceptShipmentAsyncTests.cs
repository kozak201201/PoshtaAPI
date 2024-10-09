using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.ShipmentServiceMethods
{
    public class AcceptShipmentAsyncTests : ShipmentServiceTestsBase
    {
        [Fact]
        public async Task AcceptShipmentAsync_ValidParameters_AcceptsShipment()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();

            var postOfficeTypeResult = PostOfficeType.Create(
                Guid.NewGuid(),
                "Type",
                10f,
                10f,
                10f,
                10f
            );

            Assert.True(postOfficeTypeResult.IsSuccess);
            var postOfficeType = postOfficeTypeResult.Value;

            var postOfficeResult = PostOffice.Create(
                postOfficeId,
                1,
                "City",
                "Address",
                100,
                50.0,
                30.0,
                postOfficeType
            );

            Assert.True(postOfficeResult.IsSuccess);
            var postOffice = postOfficeResult.Value;

            postOfficeServiceMock
                .Setup(x => x.GetPostOfficeByIdAsync(postOfficeId))
                .ReturnsAsync(Result.Success(postOffice));

            var shipment = Shipment.Create(
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
            ).Value;

            shipmentsRepositoryMock
                .Setup(x => x.GetByIdAsync(shipmentId))
                .ReturnsAsync(shipment);

            // Act
            var result = await shipmentService.AcceptShipmentAsync(shipmentId, postOfficeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("accepted by", result.Value);
            shipmentsRepositoryMock.Verify(x => x.UpdateStatusAsync(shipmentId, ShipmentStatus.AtPostOffice), Times.Once);
            shipmentsRepositoryMock.Verify(x => x.UpdateCurrentPostOfficeAsync(shipmentId, postOfficeId), Times.Once);
        }


        [Fact]
        public async Task AcceptShipmentAsync_ShipmentNotFound_ReturnsFailure()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();

            shipmentsRepositoryMock
                .Setup(x => x.GetByIdAsync(shipmentId))
                .ReturnsAsync((Shipment)null); // Shipment not found

            // Act
            var result = await shipmentService.AcceptShipmentAsync(shipmentId, postOfficeId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Shipment with id: {shipmentId} wasn't found", result.Error);
        }
    }
}
