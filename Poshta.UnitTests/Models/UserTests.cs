using Poshta.Core.Models;

namespace Poshta.UnitTests.Models
{
    public class UserTests
    {
        [Fact]
        public void Create_ValidUser_ReturnsUser()
        {
            // Arrange
            var id = Guid.NewGuid();
            var lastName = "TestLastName";
            var firstName = "TestFirstName";
            var passwordHash = "hashedPassword";
            var phoneNumber = "+1234567890";

            // Act
            var result = User.Create(id, lastName, firstName, passwordHash, phoneNumber);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(lastName, result.Value.LastName);
            Assert.Equal(firstName, result.Value.FirstName);
            Assert.Equal(phoneNumber, result.Value.PhoneNumber);
        }

        [Fact]
        public void Create_InvalidLastName_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var lastName = "123";
            var firstName = "TestFirstName";
            var passwordHash = "hashedPassword";
            var phoneNumber = "+1234567890";

            // Act
            var result = User.Create(id, lastName, firstName, passwordHash, phoneNumber);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid last name", result.Error);
        }

        [Fact]
        public void Create_InvalidFirstName_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var lastName = "TestLastName";
            var firstName = "123";
            var passwordHash = "hashedPassword";
            var phoneNumber = "+1234567890";

            // Act
            var result = User.Create(id, lastName, firstName, passwordHash, phoneNumber);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid first name", result.Error);
        }

        [Fact]
        public void Create_InvalidMiddleName_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var lastName = "TestLastName";
            var firstName = "TestFirstName";
            var passwordHash = "hashedPassword";
            var phoneNumber = "+1234567890";
            var middleName = "123";

            // Act
            var result = User.Create(id, lastName, firstName, passwordHash, phoneNumber, middleName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid middle name", result.Error);
        }

        [Fact]
        public void Create_InvalidPhoneNumber_ReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var lastName = "TestLastName";
            var firstName = "TestFirstName";
            var passwordHash = "hashedPassword";
            var phoneNumber = "12345";

            // Act
            var result = User.Create(id, lastName, firstName, passwordHash, phoneNumber);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid phone", result.Error);
        }

        [Fact]
        public void Create_ValidPhoneNumber_NormalizesPhoneNumber()
        {
            // Arrange
            var id = Guid.NewGuid();
            var lastName = "TestLastName";
            var firstName = "TestFirstName";
            var passwordHash = "hashedPassword";
            var phoneNumber = "(123) 456-7890";

            // Act
            var result = User.Create(id, lastName, firstName, passwordHash, phoneNumber);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("1234567890", result.Value.PhoneNumber);
        }
    }
}
