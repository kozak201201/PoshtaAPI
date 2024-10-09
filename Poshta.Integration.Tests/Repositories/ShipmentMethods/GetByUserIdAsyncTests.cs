using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class GetByUserIdAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnShipments_WhenUserIsSender()
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
                50.0).Value;

            await shipmentsRepository.CreateAsync(shipment1);
            await shipmentsRepository.CreateAsync(shipment2);

            // Act
            var shipments = await shipmentsRepository.GetByUserIdAsync(sender.Id);

            // Assert
            Assert.NotNull(shipments);
            Assert.Equal(2, shipments.Count());
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnShipments_WhenUserIsRecipient()
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
            var shipments = await shipmentsRepository.GetByUserIdAsync(recipient.Id);

            // Assert
            Assert.NotNull(shipments);
            Assert.Single(shipments);
            Assert.Equal(shipment.Id, shipments.First().Id);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnShipments_WhenUserIsConfidant()
        {
            // Arrange
            var confidantId = Guid.NewGuid();
            var confidantResult = User.Create(confidantId, "ConfidantLastName", "ConfidantFirstName", "password", "+1234567893");
            Assert.True(confidantResult.IsSuccess);
            var confidant = confidantResult.Value;
            await usersRepository.CreateAsync(confidant);

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

            shipment.ConfidantId = confidantId;

            await shipmentsRepository.CreateAsync(shipment);

            // Act
            var shipments = await shipmentsRepository.GetByUserIdAsync(confidantId);

            // Assert
            Assert.NotNull(shipments);
            Assert.Single(shipments);
            Assert.Equal(shipment.Id, shipments.First().Id);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnEmptyList_WhenNoShipmentsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var shipments = await shipmentsRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(shipments);
            Assert.Empty(shipments);
        }
    }
}
