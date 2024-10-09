using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class UpdateEndPostOfficeAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task UpdateEndPostOfficeAsync_ShouldUpdateEndPostOffice_WhenShipmentExists()
        {
            // Arrange
            var sender = await CreateTestUser1Async();
            var recipient = await CreateTestUser2Async();
            var startPostOffice = await CreateTestPostOffice1Async();
            var originalEndPostOffice = await CreateTestPostOffice2Async();

            var postOfficeType = await CreateTestPostOfficeTypeAsync();

            var newEndPostOfficeId = Guid.NewGuid();

            var newEndPostOfficeResult = PostOffice.Create(
                newEndPostOfficeId,
                4,
                "City32",
                "Address12",
                20,
                50,
                80,
                postOfficeType);

            Assert.True(newEndPostOfficeResult.IsSuccess);

            var newEndPostOffice = newEndPostOfficeResult.Value;

            await postOfficesRepository.CreateAsync(newEndPostOffice);

            var shipment = Shipment.Create(
                Guid.NewGuid(),
                sender.Id,
                recipient.Id,
                startPostOffice.Id,
                originalEndPostOffice.Id,
                PayerType.Sender,
                "12345678901234",
                50.0).Value;

            await shipmentsRepository.CreateAsync(shipment);

            // Act
            await shipmentsRepository.UpdateEndPostOfficeAsync(shipment.Id, newEndPostOffice.Id);

            // Assert
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.NotNull(updatedShipment);
            Assert.Equal(newEndPostOffice.Id, updatedShipment.EndPostOfficeId);
        }
    }

}
