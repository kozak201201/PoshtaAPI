using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.OperatorServiceMethods
{
    public class UpdatePostOfficeAsyncTests : OperatorServiceTestsBase
    {
        [Fact]
        public async Task UpdatePostOfficeAsync_ShouldReturnSuccess_WhenOperatorExists()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();
            var operatorPostOffice = Operator.Create(operatorId, Guid.NewGuid(), postOfficeId).Value;

            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync(operatorPostOffice);
            mockOperatorsRepository.Setup(x => x.UpdatePostOfficeAsync(operatorId, postOfficeId)).Returns(Task.CompletedTask);

            // Act
            var result = await operatorService.UpdatePostOfficeAsync(operatorId, postOfficeId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"Success update operator post office. operator id: {operatorId}. post office id: {postOfficeId}.", result.Value);
            mockOperatorsRepository.Verify(x => x.UpdatePostOfficeAsync(operatorId, postOfficeId), Times.Once);
        }

        [Fact]
        public async Task UpdatePostOfficeAsync_ShouldReturnFailure_WhenOperatorNotFound()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();

            mockOperatorsRepository.Setup(x => x.GetByIdAsync(operatorId)).ReturnsAsync((Operator)null);

            // Act
            var result = await operatorService.UpdatePostOfficeAsync(operatorId, postOfficeId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Operator with id: {operatorId} wasn't found", result.Error);
            mockOperatorsRepository.Verify(x => x.UpdatePostOfficeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}
