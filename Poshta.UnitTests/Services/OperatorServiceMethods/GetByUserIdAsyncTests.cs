using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.OperatorServiceMethods
{
    public class GetByUserIdAsyncTests : OperatorServiceTestsBase
    {
        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnOperator_WhenFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var operatorPostOfficeResult = Operator.Create(Guid.NewGuid(), userId, Guid.NewGuid());

            Assert.True(operatorPostOfficeResult.IsSuccess);
            var operatorPostOffice = operatorPostOfficeResult.Value;

            mockOperatorsRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(operatorPostOffice);

            // Act
            var result = await operatorService.GetByUserIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(userId, result.Value.UserId);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            mockOperatorsRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync((Operator)null);

            // Act
            var result = await operatorService.GetByUserIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Operator with user id: {userId} wasn't found", result.Error);
        }
    }
}
