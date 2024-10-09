using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class GetByPostOfficeIdAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetByPostOfficeIdAsync_ShouldReturnShipments_WhenExistInPostOffice()
        {
            // Arrange
            var sender = await CreateTestUser1Async();
            var recipient = await CreateTestUser2Async();

            var postOffice1 = await CreateTestPostOffice1Async();
            var postOffice2 = await CreateTestPostOffice2Async();

            var shipment1 = Shipment.Create(
                Guid.NewGuid(),
                sender.Id,
                recipient.Id,
                postOffice1.Id,
                postOffice2.Id,
                PayerType.Sender,
                "12345678901234",
                50.0).Value;

            var shipment2 = Shipment.Create(
                Guid.NewGuid(),
                sender.Id,
                recipient.Id,
                postOffice1.Id,
                postOffice2.Id,
                PayerType.Recipient,
                "12345678901235",
                60.0).Value;

            shipment1.CurrentPostOfficeId = postOffice1.Id;
            shipment2.CurrentPostOfficeId = postOffice1.Id;

            await shipmentsRepository.CreateAsync(shipment1);
            await shipmentsRepository.CreateAsync(shipment2);

            // Act
            var shipments = await shipmentsRepository.GetByPostOfficeIdAsync(postOffice1.Id);

            // Assert
            Assert.NotNull(shipments);
            Assert.Equal(2, shipments.Count());
            Assert.Contains(shipments, s => s.Id == shipment1.Id);
            Assert.Contains(shipments, s => s.Id == shipment2.Id);
        }

        [Fact]
        public async Task GetByPostOfficeIdAsync_ShouldReturnEmptyList_WhenNoShipmentsInPostOffice()
        {
            // Arrange
            var postOfficeId = Guid.NewGuid();

            // Act
            var shipments = await shipmentsRepository.GetByPostOfficeIdAsync(postOfficeId);

            // Assert
            Assert.NotNull(shipments);
            Assert.Empty(shipments);
        }
    }
}
