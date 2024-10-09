using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.OperatorServiceMethods
{
    public class DeleteAsyncTests : OperatorServiceTestsBase
    {
        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenOperatorExists()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();

            var operatorResult = Operator.Create(operatorId, userId, postOfficeId);
            Assert.True(operatorResult.IsSuccess);

            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync(operatorResult.Value);
            mockOperatorsRepository.Setup(x => x.DeleteAsync(operatorId)).Returns(Task.CompletedTask);

            // Act
            var result = await operatorService.DeleteAsync(operatorId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"Success delete operator with id: {operatorId}.", result.Value);
            mockOperatorsRepository.Verify(x => x.DeleteAsync(operatorId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFailure_WhenOperatorNotFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();

            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync((Operator)null);

            // Act
            var result = await operatorService.DeleteAsync(operatorId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Operator with id: {operatorId} wasn't found", result.Error);
            mockOperatorsRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
