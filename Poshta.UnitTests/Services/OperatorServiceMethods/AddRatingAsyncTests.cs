using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.OperatorServiceMethods
{
    public class AddRatingAsyncTests : OperatorServiceTestsBase
    {
        private Mock<IShipmentService> shipmentServiceMock;

        public AddRatingAsyncTests()
        {
            shipmentServiceMock = new Mock<IShipmentService>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IShipmentService))).Returns(shipmentServiceMock.Object);
        }

        [Fact]
        public async Task AddRatingAsync_ShouldReturnSuccess_WhenRatingIsValid()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var shipment = CreateShipment(shipmentId, userId, operatorId, ShipmentStatus.Delivered);

            var userResult = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+1234567890");
            var operatorResult = Operator.Create(operatorId, userId, Guid.NewGuid());

            Assert.True(userResult.IsSuccess);
            Assert.True(operatorResult.IsSuccess);

            shipmentServiceMock.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync(Result.Success(shipment));
            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult.Value);
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(Result.Success(userResult.Value));
            mockOperatorsRepository.Setup(x => x.AddRatingAsync(It.IsAny<OperatorRating>())).Returns(Task.CompletedTask);

            // Act
            var result = await operatorService.AddRatingAsync(operatorId, userId, shipmentId, 5, "Great service!");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"Success add rating for operator with id: {operatorId}." +
                $"From user with id: {userId}. " +
                $"Shipment id: {shipment.Id}", result.Value);
            mockOperatorsRepository.Verify(x => x.AddRatingAsync(It.IsAny<OperatorRating>()), Times.Once);
        }

        [Fact]
        public async Task AddRatingAsync_ShouldReturnFailure_WhenShipmentNotFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();

            shipmentServiceMock.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync(Result.Failure<Shipment>("Shipment not found"));

            // Act
            var result = await operatorService.AddRatingAsync(operatorId, userId, shipmentId, 5, "Great service!");

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Shipment not found", result.Error);
        }

        [Fact]
        public async Task AddRatingAsync_ShouldReturnFailure_WhenShipmentNotDelivered()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var shipment = CreateShipment(shipmentId, userId, operatorId, ShipmentStatus.InTransit);

            var userResult = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+1234567890");
            var operatorResult = Operator.Create(operatorId, userId, Guid.NewGuid());

            shipmentServiceMock.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync(Result.Success(shipment));
            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult.Value);
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(Result.Success(userResult.Value));

            // Act
            var result = await operatorService.AddRatingAsync(operatorId, userId, shipmentId, 5, "Great service!");

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"User with id: {userId} can't rate operator  " +
                    $"because shipment doesn't have status: Delivered", result.Error);
        }

        [Fact]
        public async Task AddRatingAsync_ShouldReturnFailure_WhenShipmentDoesntHaveOperatorWhoIssuedNotFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var shipment = CreateShipment(shipmentId, userId, null, ShipmentStatus.Delivered);

            var userResult = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+1234567890");
            var operatorResult = Operator.Create(operatorId, userId, Guid.NewGuid());

            shipmentServiceMock.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync(Result.Success(shipment));
            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync((Operator?)null);
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(Result.Success(userResult.Value));

            // Act
            var result = await operatorService.AddRatingAsync(operatorId, userId, shipmentId, 5, "Great service!");

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Operator who issued shipment with id: {shipmentId} wasn't found", result.Error);
        }

        [Fact]
        public async Task AddRatingAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var shipment = CreateShipment(shipmentId, userId, operatorId, ShipmentStatus.Delivered);

            var operatorResult = Operator.Create(operatorId, userId, Guid.NewGuid());

            shipmentServiceMock.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync(Result.Success(shipment));
            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult.Value);
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(Result.Failure<User>("User not found"));

            // Act
            var result = await operatorService.AddRatingAsync(operatorId, userId, shipmentId, 5, "Great service!");

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("User not found", result.Error);
        }

        [Fact]
        public async Task AddRatingAsync_ShouldReturnFailure_WhenOperatorDidNotTransferShipment()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var shipment = CreateShipment(shipmentId, userId, Guid.NewGuid(), ShipmentStatus.Delivered);

            var userResult = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+1234567890");
            var operatorResult = Operator.Create(operatorId, userId, Guid.NewGuid());

            shipmentServiceMock.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync(Result.Success(shipment));
            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult.Value);
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(Result.Success(userResult.Value));

            // Act
            var result = await operatorService.AddRatingAsync(operatorId, userId, shipmentId, 5, "Great service!");

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"User with id: {userId} can't rate " +
                    $"operator with id: {operatorId} because operator did not transfer " +
                    $"the shipment with id: {shipment.Id} to the recipient", result.Error);
        }

        [Fact]
        public async Task AddRatingAsync_ShouldReturnFailure_WhenUserIsNotRecipientOrConfidant()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var shipmentId = Guid.NewGuid();
            var shipment = CreateShipment(shipmentId, Guid.NewGuid(), operatorId, ShipmentStatus.Delivered); // Другой получатель

            var userResult = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+1234567890");
            var operatorResult = Operator.Create(operatorId, userId, Guid.NewGuid());

            shipmentServiceMock.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync(Result.Success(shipment));
            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult.Value);
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(Result.Success(userResult.Value));

            // Act
            var result = await operatorService.AddRatingAsync(operatorId, userId, shipmentId, 5, "Great service!");

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"User with id: {userId} isn'n confidant or recipient that's why he can't rate operator", result.Error);
        }

        [Fact]
        public async Task AddRatingAsync_ShouldReturnFailure_WhenOperatorTriesToRateHimself()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var userId = operatorId; // Оператор пытается оценить себя
            var shipmentId = Guid.NewGuid();
            var shipment = CreateShipment(shipmentId, userId, operatorId, ShipmentStatus.Delivered);

            var userResult = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+1234567890");
            var operatorResult = Operator.Create(operatorId, userId, Guid.NewGuid());

            shipmentServiceMock.Setup(s => s.GetShipmentByIdAsync(shipmentId)).ReturnsAsync(Result.Success(shipment));
            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult.Value);
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(Result.Success(userResult.Value));

            // Act
            var result = await operatorService.AddRatingAsync(operatorId, userId, shipmentId, 5, "Great service!");

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Fail. Operator with id: {operatorId} try to rate himself", result.Error);
        }

        private Shipment CreateShipment(Guid shipmentId, Guid userId, Guid? operatorId, ShipmentStatus status)
        {
            var shipment = Shipment.Create(
                shipmentId,
                Guid.NewGuid(),
                userId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                PayerType.Recipient,
                "12345678901234",
                100).Value;

            shipment.OperatorWhoIssuedId = operatorId;
            shipment.Status = status;

            return shipment;
        }
    }
}
