using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class UpdateConfidantAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task UpdateConfidantAsync_ShouldUpdateConfidant_WhenShipmentExists()
        {
            // Arrange
            var sender = await CreateTestUser1Async();
            var recipient = await CreateTestUser2Async();
            var startPostOffice = await CreateTestPostOffice1Async();
            var endPostOffice = await CreateTestPostOffice2Async();

            var confidantId = Guid.NewGuid();
            var confidantResult = User.Create(confidantId, "ConfidantLastName", "ConfidantFirstName", "password", "+1234567893");
            Assert.True(confidantResult.IsSuccess);
            var confidant = confidantResult.Value;

            await usersRepository.CreateAsync(confidant);

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
            await shipmentsRepository.UpdateConfidantAsync(shipment.Id, confidantId);

            // Assert
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.NotNull(updatedShipment);
            Assert.Equal(confidantId, updatedShipment.ConfidantId);
        }

        [Fact]
        public async Task UpdateConfidantAsync_ShouldThrowException_WhenShipmentDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            Guid? confidantId = Guid.NewGuid(); // Можете использовать null, если это требуется

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                shipmentsRepository.UpdateConfidantAsync(nonExistentId, confidantId));
            Assert.Equal($"Shipment with id: {nonExistentId} wasn't found", exception.Message);
        }
    }
}
