using CSharpFunctionalExtensions;
using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.ShipmentServiceMethods
{
    public class CreateShipmentAsyncTests : ShipmentServiceTestsBase
    {
        [Fact]
        public async Task CreateShipmentAsync_ValidInput_CreatesShipment()
        {
            // Arrange
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var startPostOfficeId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();
            var payer = PayerType.Sender;
            var appraisedValue = Shipment.MIN_APPRAISED_VALUE;
            var weight = Shipment.DEFAULT_WEIGHT;

            var postOfficeType = PostOfficeType.Create(Guid.NewGuid(), "Standard", 20, 50, 30, 15).Value;

            var startPostOffice = PostOffice.Create(startPostOfficeId, 1, "CityA", "AddressA", 15, 0, 0, postOfficeType).Value;
            var endPostOffice = PostOffice.Create(endPostOfficeId, 2, "CityB", "AddressB", 15, 0, 0, postOfficeType).Value;

            postOfficeServiceMock.Setup(x => x.GetPostOfficeByIdAsync(startPostOfficeId)).ReturnsAsync(Result.Success(startPostOffice));
            postOfficeServiceMock.Setup(x => x.GetPostOfficeByIdAsync(endPostOfficeId)).ReturnsAsync(Result.Success(endPostOffice));

            // Act
            var result = await shipmentService.CreateShipmentAsync(senderId, recipientId, startPostOfficeId, endPostOfficeId, payer, appraisedValue, weight);

            // Assert
            Assert.True(result.IsSuccess);
            shipmentsRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Shipment>()), Times.Once);
        }

        [Fact]
        public async Task CreateShipmentAsync_StartPostOfficeNotFound_ReturnsFailure()
        {
            // Arrange
            var startPostOfficeId = Guid.NewGuid();
            postOfficeServiceMock.Setup(x => x.GetPostOfficeByIdAsync(startPostOfficeId)).ReturnsAsync(Result.Failure<PostOffice>("Start post office not found"));

            // Act
            var result = await shipmentService.CreateShipmentAsync(Guid.NewGuid(), Guid.NewGuid(), startPostOfficeId, Guid.NewGuid(), PayerType.Sender);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Start post office not found", result.Error);
        }

        [Fact]
        public async Task CreateShipmentAsync_EndPostOfficeNotFound_ReturnsFailure()
        {
            // Arrange
            var startPostOfficeId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();
            postOfficeServiceMock.Setup(x => x.GetPostOfficeByIdAsync(startPostOfficeId)).ReturnsAsync(Result.Success(PostOffice.Create(startPostOfficeId, 1, "City", "Address", 15, 0, 0, PostOfficeType.Create(Guid.NewGuid(), "Standard", 20, 50, 30, 15).Value).Value));
            postOfficeServiceMock.Setup(x => x.GetPostOfficeByIdAsync(endPostOfficeId)).ReturnsAsync(Result.Failure<PostOffice>("End post office not found"));

            // Act
            var result = await shipmentService.CreateShipmentAsync(Guid.NewGuid(), Guid.NewGuid(), startPostOfficeId, endPostOfficeId, PayerType.Sender);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("End post office not found", result.Error);
        }
    }
}
