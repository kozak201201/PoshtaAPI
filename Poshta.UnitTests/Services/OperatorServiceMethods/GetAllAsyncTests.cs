using Moq;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services.OperatorServiceMethods
{
    public class GetAllAsyncTests : OperatorServiceTestsBase
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnOperators_WhenFound()
        {
            // Arrange
            var operator1 = Operator.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
            var operator2 = Operator.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
            var operatorsList = new List<Operator> { operator1, operator2 };

            mockOperatorsRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(operatorsList);

            // Act
            var result = await operatorService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, op => op.Id == operator1.Id);
            Assert.Contains(result, op => op.Id == operator2.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoOperatorsFound()
        {
            // Arrange
            var operatorsList = new List<Operator>();

            mockOperatorsRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(operatorsList);

            // Act
            var result = await operatorService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
