using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.ShipmentServiceMethods
{
    public class RedirectShipmentAsyncTests : ShipmentServiceTestsBase
    {
        [Fact]
        public async Task RedirectShipmentAsync_ShouldRedirectShipment_WhenConditionsAreMet()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();

            var newEndPostOfficeId = Guid.NewGuid();
            var shipmentResult = Shipment.Create(
                shipmentId,
                userId,
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

            shipmentsRepositoryMock.Setup(x => x.GetByIdAsync(shipmentId))
                .ReturnsAsync(shipment);

            var operatorResult = Operator.Create(Guid.NewGuid(), Guid.NewGuid(), postOfficeId);
            operatorServiceMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(operatorResult);

            var postOfficeTypeResult = PostOfficeType.Create(
                Guid.NewGuid(),
                "Type A",
                100.0f,
                100.0f,
                50.0f,
                50.0f
            );

            Assert.True(postOfficeTypeResult.IsSuccess);
            var postOfficeType = postOfficeTypeResult.Value;

            var newPostOfficeResult = PostOffice.Create(
                Guid.NewGuid(),
                2,
                "Los Angeles",
                "456 Elm St",
                100,
                34.0522,
                -118.2437,
                postOfficeType
            );

            Assert.True(newPostOfficeResult.IsSuccess);
            var newPostOffice = newPostOfficeResult.Value;

            postOfficeServiceMock.Setup(x => x.GetPostOfficeByIdAsync(newEndPostOfficeId))
                .ReturnsAsync(Result.Success(newPostOffice));

            // Act
            var result = await shipmentService.RedirectShipmentAsync(userId, shipmentId, newEndPostOfficeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains($"Shipment: {shipment.TrackingNumber} update end post office successfully. " +
                $"New post office: {newPostOffice}", result.Value);
        }

    }
}
