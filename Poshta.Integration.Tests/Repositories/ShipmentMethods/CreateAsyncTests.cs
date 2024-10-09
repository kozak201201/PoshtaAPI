using Microsoft.EntityFrameworkCore;
using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class CreateAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task CreateAsync_ShouldAddShipment_WhenUsersExist()
        {
            // Arrange
            var shipmentId = Guid.NewGuid();
            var trackingNumber = "12345678901234";
            var price = 50.0;
            var appraisedValue = 200.0;

            var sender = await CreateTestUser1Async();
            var recipient = await CreateTestUser2Async();

            var startPostOffice = await CreateTestPostOffice1Async();
            var endPostOffice = await CreateTestPostOffice2Async();

            var shipment = Shipment.Create(
                shipmentId,
                sender.Id,
                recipient.Id,
                startPostOffice.Id,
                endPostOffice.Id,
                PayerType.Sender,
                trackingNumber,
                price,
                appraisedValue).Value;


            // Act
            await shipmentsRepository.CreateAsync(shipment);

            // Assert
            var createdShipment = await context.Shipments.FindAsync(shipmentId);
            Assert.NotNull(createdShipment);
            Assert.Equal(shipmentId, createdShipment.Id);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenUsersDoNotExist()
        {
            // Arrange
            var shipmentId = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();

            var startPostOffice = await CreateTestPostOffice1Async();
            var endPostOffice = await CreateTestPostOffice2Async();

            var shipment = Shipment.Create(
                shipmentId,
                senderId,
                recipientId,
                startPostOffice.Id,
                endPostOffice.Id,
                PayerType.Sender,
                "12345678901234",
                50.0).Value;

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await shipmentsRepository.CreateAsync(shipment));
        }
    }
}
