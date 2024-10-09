using Poshta.Core.Models;

namespace Poshta.UnitTests.Models
{
    public class ShipmentTests
    {
        [Fact]
        public void Create_ValidShipment_ReturnsSuccessResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var startPostOfficeId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();
            var payer = PayerType.Sender;
            var trackingNumber = "12345678901234"; // 14 characters
            var price = 100.0;
            var appraisedValue = 150.0;
            var weight = 1.0f;
            var length = 20.0f;
            var width = 15.0f;
            var height = 12.0f;

            // Act
            var result = Shipment.Create(
                id, senderId, recipientId, startPostOfficeId, endPostOfficeId,
                payer, trackingNumber, price, appraisedValue, weight, length, width, height);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(id, result.Value.Id);
            Assert.Equal(senderId, result.Value.SenderId);
            Assert.Equal(recipientId, result.Value.RecipientId);
            Assert.Equal(startPostOfficeId, result.Value.StartPostOfficeId);
            Assert.Equal(endPostOfficeId, result.Value.EndPostOfficeId);
            Assert.Equal(payer, result.Value.Payer);
            Assert.Equal(trackingNumber, result.Value.TrackingNumber);
            Assert.Equal(price, result.Value.Price);
            Assert.Equal(appraisedValue, result.Value.AppraisedValue);
            Assert.Equal(weight, result.Value.Weight);
            Assert.Equal(length, result.Value.Length);
            Assert.Equal(width, result.Value.Width);
            Assert.Equal(height, result.Value.Height);
        }

        [Theory]
        [InlineData(39.0)] // Less than MIN_PRICE
        [InlineData(-10.0)] // Negative price
        public void Create_InvalidPrice_ReturnsFailureResult(double invalidPrice)
        {
            // Arrange
            var id = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var startPostOfficeId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();
            var payer = PayerType.Sender;
            var trackingNumber = "12345678901234"; // 14 characters
            var appraisedValue = 150.0;
            var weight = 1.0f;
            var length = 20.0f;
            var width = 15.0f;
            var height = 12.0f;

            // Act
            var result = Shipment.Create(
                id, senderId, recipientId, startPostOfficeId, endPostOfficeId,
                payer, trackingNumber, invalidPrice, appraisedValue, weight, length, width, height);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("min price cannot be less then 40", result.Error);
        }

        [Theory]
        [InlineData(0.0f)] // Less than MIN_WEIGHT
        [InlineData(-1.0f)] // Negative weight
        public void Create_InvalidWeight_ReturnsFailureResult(float invalidWeight)
        {
            // Arrange
            var id = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var startPostOfficeId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();
            var payer = PayerType.Sender;
            var trackingNumber = "12345678901234"; // 14 characters
            var appraisedValue = 150.0;

            // Act
            var result = Shipment.Create(
                id, senderId, recipientId, startPostOfficeId, endPostOfficeId,
                payer, trackingNumber, 100.0, appraisedValue, invalidWeight, 20.0f, 15.0f, 12.0f);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"weight cannot be less then 0,1", result.Error);
        }

        [Theory]
        [InlineData(15.0f)] // Valid length
        [InlineData(14.9f)] // Less than MIN_LENGTH
        public void Create_InvalidLength_ReturnsFailureResult(float invalidLength)
        {
            var id = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var startPostOfficeId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();
            var payer = PayerType.Sender;
            var trackingNumber = "12345678901234"; // 14 characters
            var appraisedValue = 150.0;
            var weight = 1.0f;
            var price = 100.0;
            var width = 15.0f;
            var height = 12.0f;

            var result = Shipment.Create(
                id, senderId, recipientId, startPostOfficeId, endPostOfficeId,
                payer, trackingNumber, price, appraisedValue, weight, invalidLength, width, height);

            Assert.False(result.IsSuccess);
            Assert.Equal("length cannot be less then 16", result.Error);
        }

        [Theory]
        [InlineData(10.9f)] // Less than MIN_WIDTH
        public void Create_InvalidWidth_ReturnsFailureResult(float invalidWidth)
        {
            var id = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var startPostOfficeId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();
            var payer = PayerType.Sender;
            var trackingNumber = "12345678901234"; // 14 characters
            var appraisedValue = 150.0;
            var weight = 1.0f;
            var price = 100.0;
            var length = 20.0f;
            var height = 12.0f;

            var result = Shipment.Create(
                id, senderId, recipientId, startPostOfficeId, endPostOfficeId,
                payer, trackingNumber, price, appraisedValue, weight, length, invalidWidth, height);

            Assert.False(result.IsSuccess);
            Assert.Equal("width cannot be less then 11", result.Error);
        }

        [Theory]
        [InlineData(9.9f)] // Less than MIN_HEIGHT
        public void Create_InvalidHeight_ReturnsFailureResult(float invalidHeight)
        {
            var id = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var startPostOfficeId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();
            var payer = PayerType.Sender;
            var trackingNumber = "12345678901234"; // 14 characters
            var appraisedValue = 150.0;
            var weight = 1.0f;
            var price = 100.0;
            var length = 20.0f;
            var width = 15.0f;

            var result = Shipment.Create(
                id, senderId, recipientId, startPostOfficeId, endPostOfficeId,
                payer, trackingNumber, price, appraisedValue, weight, length, width, invalidHeight);

            Assert.False(result.IsSuccess);
            Assert.Equal("height value cannot be less then 10", result.Error);
        }

        [Theory]
        [InlineData(50.0)] // Less than MIN_APPRAISED_VALUE
        [InlineData(-10.0)] // Negative appraised value
        public void Create_InvalidAppraisedValue_ReturnsFailureResult(double invalidAppraisedValue)
        {
            var id = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();
            var startPostOfficeId = Guid.NewGuid();
            var endPostOfficeId = Guid.NewGuid();
            var payer = PayerType.Sender;
            var trackingNumber = "12345678901234"; // 14 characters
            var price = 100.0;
            var weight = 1.0f;
            var length = 20.0f;
            var width = 15.0f;
            var height = 12.0f;

            var result = Shipment.Create(
                id, senderId, recipientId, startPostOfficeId, endPostOfficeId,
                payer, trackingNumber, price, invalidAppraisedValue, weight, length, width, height);

            Assert.False(result.IsSuccess);
            Assert.Equal("appraised value cannot be less then 100", result.Error);
        }
    }
}
