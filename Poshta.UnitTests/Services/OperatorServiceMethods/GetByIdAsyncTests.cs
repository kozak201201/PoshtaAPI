using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.OperatorServiceMethods
{
    public class GetByIdAsyncTests : OperatorServiceTestsBase
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnOperator_WhenFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var operatorPostOfficeResult = Operator.Create(operatorId, Guid.NewGuid(), Guid.NewGuid());

            Assert.True(operatorPostOfficeResult.IsSuccess);
            var operatorPostOffice = operatorPostOfficeResult.Value;

            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync(operatorPostOffice);

            // Act
            var result = await operatorService.GetByIdAsync(operatorId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(operatorId, result.Value.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync((Operator)null);

            // Act
            var result = await operatorService.GetByIdAsync(operatorId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Operator with id: {operatorId} wasn't found", result.Error);
        }
    }
}
