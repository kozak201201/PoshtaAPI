using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Models
{
    public class OperatorTests
    {
        [Fact]
        public void Operator_Constructor_InitializesProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();

            // Act
            var operatorPostOfficeResult = Operator.Create(id, userId, postOfficeId);

            // Assert
            Assert.True(operatorPostOfficeResult.IsSuccess);
            var operatorPostOffice = operatorPostOfficeResult.Value;
            Assert.Equal(id, operatorPostOffice.Id);
            Assert.Equal(userId, operatorPostOffice.UserId);
            Assert.Equal(postOfficeId, operatorPostOffice.PostOfficeId);
            Assert.Empty(operatorPostOffice.Ratings);
        }

        [Fact]
        public void Ratings_Initially_Empty()
        {
            // Arrange
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var postOfficeId = Guid.NewGuid();
            var operatorPostOfficeResult = Operator.Create(id, userId, postOfficeId);

            Assert.True(operatorPostOfficeResult.IsSuccess);

            var operatorPostOffice = operatorPostOfficeResult.Value;

            // Act
            var ratings = operatorPostOffice.Ratings;

            // Assert
            Assert.NotNull(ratings);
            Assert.Empty(ratings);
        }
    }
}
