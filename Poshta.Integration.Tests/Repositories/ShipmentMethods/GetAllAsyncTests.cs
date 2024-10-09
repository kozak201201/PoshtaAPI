using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class GetAllAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllShipments_WhenExist()
        {
            // Arrange
            var sender = await CreateTestUser1Async();
            var recipient = await CreateTestUser2Async();

            var startPostOffice = await CreateTestPostOffice1Async();
            var endPostOffice = await CreateTestPostOffice2Async();

            var shipment1 = Shipment.Create(
                Guid.NewGuid(),
                sender.Id,
                recipient.Id,
                startPostOffice.Id,
                endPostOffice.Id,
                PayerType.Sender,
                "12345678901234",
                50.0).Value;

            var shipment2 = Shipment.Create(
                Guid.NewGuid(),
                sender.Id,
                recipient.Id,
                startPostOffice.Id,
                endPostOffice.Id,
                PayerType.Sender,
                "12345678901235",
                60.0).Value;

            await shipmentsRepository.CreateAsync(shipment1);
            await shipmentsRepository.CreateAsync(shipment2);

            // Act
            var shipments = await shipmentsRepository.GetAllAsync();

            // Assert
            Assert.NotNull(shipments);
            Assert.Equal(2, shipments.Count());
            Assert.Contains(shipments, s => s.Id == shipment1.Id);
            Assert.Contains(shipments, s => s.Id == shipment2.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoShipmentsExist()
        {
            // Act
            var shipments = await shipmentsRepository.GetAllAsync();

            // Assert
            Assert.NotNull(shipments);
            Assert.Empty(shipments);
        }
    }
}
