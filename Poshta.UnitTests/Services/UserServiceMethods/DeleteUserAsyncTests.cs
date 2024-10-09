using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.UserServiceMethods
{
    public class DeleteUserAsyncTests : UserServiceTestsBase
    {
        [Fact]
        public async Task DeleteUserAsync_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            usersRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"User with id: {userId} wasn't found", result.Error);
        }

        [Fact]
        public async Task DeleteUserAsync_UserIsOperator_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();

            var user = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+123456789").Value;

            // create operator
            var operatorResult = Operator.Create(operatorId, userId, postOfficeId);
            Assert.True(operatorResult.IsSuccess);

            usersRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var operatorService = new Mock<IOperatorService>();
            operatorService.Setup(os => os.GetByUserIdAsync(userId))
                .ReturnsAsync(operatorResult);

            serviceProvider.Setup(sp => sp.GetService(typeof(IOperatorService)))
                .Returns(operatorService.Object);

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("User cannot be deleted because they are an operator", result.Error);
        }

        [Fact]
        public async Task DeleteUserAsync_UserIsInvolvedInShipments_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+123456789").Value;

            var shipmentId = Guid.NewGuid();
            var shipmentResult = Shipment.Create(
                shipmentId,
                userId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                PayerType.Sender,
                "12345678901234",
                100,
                150
            );

            usersRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var operatorService = new Mock<IOperatorService>();
            operatorService.Setup(os => os.GetByUserIdAsync(userId))
                .ReturnsAsync(Result.Failure<Operator>("User is not an operator"));

            var shipmentService = new Mock<IShipmentService>();
            shipmentService.Setup(ss => ss.GetShipmentsByUserIdAsync(userId))
                .ReturnsAsync(new List<Shipment> { shipmentResult.Value });

            serviceProvider.Setup(sp => sp.GetService(typeof(IShipmentService)))
                .Returns(shipmentService.Object);

            serviceProvider.Setup(sp => sp.GetService(typeof(IOperatorService)))
                .Returns(operatorService.Object);

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("User cannot be deleted because they are involved in shipments", result.Error);
        }

        [Fact]
        public async Task DeleteUserAsync_UserSuccessfullyDeleted_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = User.Create(userId, "TestLastName", "TestFirstName", "hashedPassword", "+123456789").Value;

            usersRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var operatorService = new Mock<IOperatorService>();
            operatorService.Setup(os => os.GetByUserIdAsync(userId))
                .ReturnsAsync(Result.Failure<Operator>("User is not an operator")); //isn't operator

            serviceProvider.Setup(sp => sp.GetService(typeof(IOperatorService)))
                .Returns(operatorService.Object);

            var shipmentService = new Mock<IShipmentService>();
            shipmentService.Setup(ss => ss.GetShipmentsByUserIdAsync(userId))
                .ReturnsAsync(new List<Shipment>()); // doesn't have shipments

            serviceProvider.Setup(sp => sp.GetService(typeof(IShipmentService)))
                .Returns(shipmentService.Object);

            usersRepository.Setup(repo => repo.DeleteAsync(userId))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"User with id: {userId} removed successfully", result.Value);
        }
    }
}
