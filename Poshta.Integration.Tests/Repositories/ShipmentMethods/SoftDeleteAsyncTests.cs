using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class SoftDeleteAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task SoftDeleteAsync_ShouldMarkAsDeletedBySender_WhenSenderDeletes()
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
            await shipmentsRepository.SoftDeleteAsync(shipment.Id, sender.Id);

            // Assert
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.True(updatedShipment.IsDeletedBySender);
            Assert.False(updatedShipment.IsDeletedByRecipient);
            Assert.False(updatedShipment.IsDeletedByConfidant);
        }

        [Fact]
        public async Task SoftDeleteAsync_ShouldMarkAsDeletedByRecipient_WhenRecipientDeletes()
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
            await shipmentsRepository.SoftDeleteAsync(shipment.Id, recipient.Id);

            // Assert
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.False(updatedShipment.IsDeletedBySender);
            Assert.True(updatedShipment.IsDeletedByRecipient);
            Assert.False(updatedShipment.IsDeletedByConfidant);
        }

        [Fact]
        public async Task SoftDeleteAsync_ShouldMarkAsDeletedByConfidant_WhenConfidantDeletes()
        {
            // Arrange
            var sender = await CreateTestUser1Async();
            var recipient = await CreateTestUser2Async();

            var confidantId = Guid.NewGuid();
            var confidantResult = User.Create(confidantId, "ConfidantLastName", "ConfidantFirstName", "password", "+1234567893");
            Assert.True(confidantResult.IsSuccess);
            var confidant = confidantResult.Value;

            await usersRepository.CreateAsync(confidant);

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
            await shipmentsRepository.SoftDeleteAsync(shipment.Id, confidantId);

            // Assert
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.False(updatedShipment.IsDeletedBySender);
            Assert.False(updatedShipment.IsDeletedByRecipient);
            Assert.True(updatedShipment.IsDeletedByConfidant);
        }

        [Fact]
        public async Task SoftDeleteAsync_ShouldRemoveShipment_WhenAllPartiesHaveDeleted()
        {
            // Arrange
            var sender = await CreateTestUser1Async();
            var recipient = await CreateTestUser2Async();

            var confidantId = Guid.NewGuid();
            var confidantResult = User.Create(confidantId, "ConfidantLastName", "ConfidantFirstName", "password", "+1234567893");
            Assert.True(confidantResult.IsSuccess);
            var confidant = confidantResult.Value;

            await usersRepository.CreateAsync(confidant);

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
            await shipmentsRepository.SoftDeleteAsync(shipment.Id, sender.Id);
            await shipmentsRepository.SoftDeleteAsync(shipment.Id, recipient.Id);
            await shipmentsRepository.SoftDeleteAsync(shipment.Id, confidantId);

            // Assert
            var deletedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.Null(deletedShipment); // Must be deleted
        }

        [Fact]
        public async Task SoftDeleteAsync_ShouldNotRemoveShipment_WhenNotAllPartiesHaveDeleted()
        {
            // Arrange
            var sender = await CreateTestUser1Async();
            var recipient = await CreateTestUser2Async();

            var confidantId = Guid.NewGuid();
            var confidantResult = User.Create(confidantId, "ConfidantLastName", "ConfidantFirstName", "password", "+1234567893");
            Assert.True(confidantResult.IsSuccess);
            var confidant = confidantResult.Value;
            await usersRepository.CreateAsync(confidant);

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
            await shipmentsRepository.SoftDeleteAsync(shipment.Id, sender.Id);
            await shipmentsRepository.SoftDeleteAsync(shipment.Id, recipient.Id);
            // Don't delete confidant

            // Assert
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.NotNull(updatedShipment); // must exist
            Assert.True(updatedShipment.IsDeletedBySender);
            Assert.True(updatedShipment.IsDeletedByRecipient);
            Assert.False(updatedShipment.IsDeletedByConfidant);
        }
    }
}
