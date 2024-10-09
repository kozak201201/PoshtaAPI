using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class UpdatePaidStatusAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task UpdatePaidStatusAsync_ShouldUpdateStatus_WhenShipmentExists()
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
            await shipmentsRepository.UpdatePaidStatusAsync(shipment.Id, true);
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);

            // Assert
            Assert.NotNull(updatedShipment);
            Assert.True(updatedShipment.IsPaid);
        }
    }
}
