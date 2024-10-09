using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class UpdateStatusAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenShipmentExists()
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
            var newStatus = ShipmentStatus.Delivered;
            await shipmentsRepository.UpdateStatusAsync(shipment.Id, newStatus);

            // Assert
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.NotNull(updatedShipment);
            Assert.Equal(newStatus, updatedShipment.Status);
        }
    }
}
