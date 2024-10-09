using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.ShipmentServiceMethods
{
    public class ReceiveShipmentAsyncTests : ShipmentServiceTestsBase
    {
        [Fact]
        public async Task ReceiveShipmentAsync_ShouldSucceed_WhenValidDataProvided()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();

            var operatorResult = Operator.Create(operatorId, Guid.NewGuid(), endPostOfficeId);

            var userResult = User.Create(recipientId, "TestLastName", "TestFirstName", "hashed_password", "+1234567890");

            var shipmentResult = Shipment.Create(
                shipmentId,
                Guid.NewGuid(),
                recipientId,
                Guid.NewGuid(),
                endPostOfficeId,
                PayerType.Sender,
                "12345678901234",
                100.0,
                100.0,
                1.0f,
                20.0f,
                15.0f,
                10.0f);

            Assert.True(operatorResult.IsSuccess);
            Assert.True(userResult.IsSuccess);
            Assert.True(shipmentResult.IsSuccess);

            var shipment = shipmentResult.Value;
            shipment.CurrentPostOfficeId = endPostOfficeId;
            shipment.IsPaid = true;

            operatorServiceMock.Setup(o => o.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult);
            userServiceMock.Setup(u => u.GetUserByIdAsync(recipientId)).ReturnsAsync(userResult);
            shipmentsRepositoryMock.Setup(repo => repo.GetByIdAsync(shipmentId)).ReturnsAsync(shipment);

            // Act
            var result = await shipmentService.ReceiveShipmentAsync(recipientId, shipmentId, operatorId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("was receive successfully", result.Value);
        }

        [Fact]
        public async Task ReceiveShipmentAsync_ShouldFail_WhenOperatorNotFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();

            var userResult = User.Create(recipientId, "TestLastName", "TestFirstName", "hashed_password", "+1234567890");
            var shipmentResult = Shipment.Create(
                shipmentId,
                Guid.NewGuid(),
                recipientId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                PayerType.Sender,
                "12345678901234",
                100.0,
                100.0,
                1.0f,
                20.0f,
                15.0f,
                10.0f);

            Assert.True(userResult.IsSuccess);
            Assert.True(shipmentResult.IsSuccess);

            userServiceMock.Setup(u => u.GetUserByIdAsync(recipientId)).ReturnsAsync(userResult);
            shipmentsRepositoryMock.Setup(repo => repo.GetByIdAsync(shipmentId)).ReturnsAsync(shipmentResult.Value);

            operatorServiceMock.Setup(o => o.GetByIdAsync(operatorId)).ReturnsAsync(Result.Failure<Operator>("Operator not found"));

            // Act
            var result = await shipmentService.ReceiveShipmentAsync(recipientId, shipmentId, operatorId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Operator not found", result.Error);
        }

        [Fact]
        public async Task ReceiveShipmentAsync_ShouldFail_WhenShipmentNotFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();

            var operatorResult = Operator.Create(operatorId, Guid.NewGuid(), Guid.NewGuid());
            var userResult = User.Create(recipientId, "TestLastName", "TestFirstName", "hashed_password", "+1234567890");

            Assert.True(operatorResult.IsSuccess);
            Assert.True(userResult.IsSuccess);

            operatorServiceMock.Setup(o => o.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult);
            userServiceMock.Setup(u => u.GetUserByIdAsync(recipientId)).ReturnsAsync(userResult);
            shipmentsRepositoryMock.Setup(repo => repo.GetByIdAsync(shipmentId)).ReturnsAsync((Shipment)null);

            // Act
            var result = await shipmentService.ReceiveShipmentAsync(recipientId, shipmentId, operatorId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Shipment with id: {shipmentId} wasn't found", result.Error);
        }

        [Fact]
        public async Task ReceiveShipmentAsync_ShouldFail_WhenCurrentPostOfficeNotFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();

            var operatorResult = Operator.Create(operatorId, Guid.NewGuid(), Guid.NewGuid());
            var userResult = User.Create(recipientId, "TestLastName", "TestFirstName", "hashed_password", "+1234567890");
            var shipmentResult = Shipment.Create(
                shipmentId,
                Guid.NewGuid(),
                recipientId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                PayerType.Sender,
                "12345678901234",
                100.0,
                100.0,
                1.0f,
                20.0f,
                15.0f,
                10.0f);

            Assert.True(operatorResult.IsSuccess);
            Assert.True(userResult.IsSuccess);

            var shipment = shipmentResult.Value;
            shipment.CurrentPostOfficeId = null;

            operatorServiceMock.Setup(o => o.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult);
            userServiceMock.Setup(u => u.GetUserByIdAsync(recipientId)).ReturnsAsync(userResult);
            shipmentsRepositoryMock.Setup(repo => repo.GetByIdAsync(shipmentId)).ReturnsAsync(shipment);

            // Act
            var result = await shipmentService.ReceiveShipmentAsync(recipientId, shipmentId, operatorId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Current post office wasn't found", result.Error);
        }

        [Fact]
        public async Task ReceiveShipmentAsync_ShouldFail_WhenOperatorNotAtCurrentPostOffice()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();

            var operatorResult = Operator.Create(operatorId, Guid.NewGuid(), Guid.NewGuid());
            var userResult = User.Create(recipientId, "TestLastName", "TestFirstName", "hashed_password", "+1234567890");
            var shipmentResult = Shipment.Create(
                shipmentId,
                Guid.NewGuid(),
                recipientId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                PayerType.Sender,
                "12345678901234",
                100.0,
                100.0,
                1.0f,
                20.0f,
                15.0f,
                10.0f);

            Assert.True(operatorResult.IsSuccess);
            Assert.True(userResult.IsSuccess);

            var shipment = shipmentResult.Value;
            var operatorPostOfficeId = Guid.NewGuid();
            shipment.CurrentPostOfficeId = operatorPostOfficeId;
            shipment.IsPaid = true;

            operatorServiceMock.Setup(o => o.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult);
            userServiceMock.Setup(u => u.GetUserByIdAsync(recipientId)).ReturnsAsync(userResult);
            shipmentsRepositoryMock.Setup(repo => repo.GetByIdAsync(shipmentId)).ReturnsAsync(shipment);

            // Act
            var result = await shipmentService.ReceiveShipmentAsync(recipientId, shipmentId, operatorId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Operator with id: {operatorResult.Value.Id} doesn't work " +
                         $"at this post office and can't receive shipment with id: {shipmentId}.", result.Error);
        }
    }
}
