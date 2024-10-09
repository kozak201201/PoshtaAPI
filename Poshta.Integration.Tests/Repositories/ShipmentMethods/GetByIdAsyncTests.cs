using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class GetByIdAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnShipment_WhenExists()
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
            var retrievedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);

            // Assert
            Assert.NotNull(retrievedShipment);
            Assert.Equal(shipment.Id, retrievedShipment.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var retrievedShipment = await shipmentsRepository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(retrievedShipment);
        }
    }
}
