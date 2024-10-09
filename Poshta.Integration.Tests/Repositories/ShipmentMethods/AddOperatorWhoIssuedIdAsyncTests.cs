using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.ShipmentMethods
{
    public class AddOperatorWhoIssuedIdTests : RepositoryTestsBase
    {
        [Fact]
        public async Task AddOperatorWhoIssuedId_ShouldUpdateOperatorWhoIssuedId_WhenShipmentExists()
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

            var operatorId = Guid.NewGuid();

            //you can create operator with another id. Not sender.
            var operatorResult = Operator.Create(operatorId, sender.Id, endPostOffice.Id);

            Assert.True(operatorResult.IsSuccess);
            var operatorPostOffice = operatorResult.Value;

            await operatorsRepository.CreateAsync(operatorPostOffice);

            // Act
            await shipmentsRepository.AddOperatorWhoIssuedId(shipment.Id, operatorId);

            // Assert
            var updatedShipment = await shipmentsRepository.GetByIdAsync(shipment.Id);
            Assert.NotNull(updatedShipment);
            Assert.Equal(operatorId, updatedShipment.OperatorWhoIssuedId);
        }
    }
}
