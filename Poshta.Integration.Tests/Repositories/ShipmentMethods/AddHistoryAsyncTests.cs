using Microsoft.EntityFrameworkCore;
using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class AddHistoryAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task AddHistoryAsync_ShouldAddHistory_WhenShipmentExists()
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

            var shipmentHistoryResult = ShipmentHistory.Create(
                Guid.NewGuid(),
                shipment.Id,
                ShipmentStatus.InTransit,
                startPostOffice.Id,
                DateTime.UtcNow,
                "Shipment is in transit");

            Assert.True(shipmentHistoryResult.IsSuccess);

            // Act
            await shipmentsRepository.AddHistoryAsync(shipment.Id, shipmentHistoryResult.Value);

            // Assert
            var histories = await context.ShipmentHistories
                .Where(h => h.ShipmentId == shipment.Id)
                .ToListAsync();

            Assert.NotNull(histories);
            Assert.Single(histories);
            Assert.Equal(shipmentHistoryResult.Value.Description, histories.First().Description);
            Assert.Equal(shipmentHistoryResult.Value.Status, histories.First().Status);
        }

        [Fact]
        public async Task AddHistoryAsync_ShouldThrowException_WhenShipmentDoesNotExist()
        {
            // Arrange
            var nonExistentShipmentId = Guid.NewGuid();
            var shipmentHistoryResult = ShipmentHistory.Create(
                Guid.NewGuid(),
                nonExistentShipmentId,
                ShipmentStatus.InTransit,
                Guid.NewGuid(),
                DateTime.UtcNow,
                "Shipment is in transit");

            Assert.True(shipmentHistoryResult.IsSuccess);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
                shipmentsRepository.AddHistoryAsync(nonExistentShipmentId, shipmentHistoryResult.Value));
        }

        [Fact]
        public async Task AddHistoryAsync_ShouldThrowException_WhenDescriptionIsNullOrEmpty()
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

            var shipmentHistoryResult = ShipmentHistory.Create(
                Guid.NewGuid(),
                shipment.Id,
                ShipmentStatus.InTransit,
                startPostOffice.Id,
                DateTime.UtcNow,
                string.Empty);

            Assert.False(shipmentHistoryResult.IsSuccess);
            Assert.Equal("description can't be null or empty", shipmentHistoryResult.Error);
        }
    }

}
