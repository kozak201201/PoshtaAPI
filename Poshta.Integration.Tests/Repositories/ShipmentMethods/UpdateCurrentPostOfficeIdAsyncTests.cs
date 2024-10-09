using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class UpdateCurrentPostOfficeAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task UpdateCurrentPostOfficeAsync_ShouldUpdateCurrentPostOffice_WhenShipmentExists()
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

            var postOfficeType = await CreateTestPostOfficeTypeAsync();

            var newPostOfficeId = Guid.NewGuid();

            var newPostOfficeResult = PostOffice.Create(
                newPostOfficeId,
                4,
                "City32",
                "Address12",
                20,
                50,
                80,
                postOfficeType);

            Assert.True(newPostOfficeResult.IsSuccess);

            var newPostOffice = newPostOfficeResult.Value;

            await postOfficesRepository.CreateAsync(newPostOffice);

            // Act
            await shipmentsRepository.UpdateCurrentPostOfficeAsync(shipment.Id, newPostOfficeId);

            // Assert
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.NotNull(updatedShipment);
            Assert.Equal(newPostOfficeId, updatedShipment.CurrentPostOfficeId);
        }
    }
}
