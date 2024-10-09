using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class GetByTrackingNumberAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetByTrackingNumberAsync_ShouldReturnShipment_WhenExists()
        {
            // Arrange
            var sender = await CreateTestUser1Async();
            var recipient = await CreateTestUser2Async();

            var startPostOffice = await CreateTestPostOffice1Async();
            var endPostOffice = await CreateTestPostOffice2Async();

            var shipment = Shipment.Create(
                Guid.NewGuid(),
                sender.Id,
                recipient.Id,
                startPostOffice.Id,
                endPostOffice.Id,
                PayerType.Sender,
                "12345678901234",
                50.0).Value;

            await shipmentsRepository.CreateAsync(shipment);

            // Act
            var retrievedShipment = await shipmentsRepository.GetByTrackingNumberAsync("12345678901234");

            // Assert
            Assert.NotNull(retrievedShipment);
            Assert.Equal(shipment.Id, retrievedShipment.Id);
        }

        [Fact]
        public async Task GetByTrackingNumberAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var trackingNumber = "nonexistent_tracking_number";

            // Act
            var retrievedShipment = await shipmentsRepository.GetByTrackingNumberAsync(trackingNumber);

            // Assert
            Assert.Null(retrievedShipment);
        }
    }
}
