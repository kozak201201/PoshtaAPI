using Poshta.Core.Exceptions;
using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.PostOfficeMethods
{
    public class CreateAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task CreateAsync_ValidPostOffice_CreatesPostOffice()
        {
            // Arrange
            var postOfficeType = await CreateTestPostOfficeTypeAsync();

            var postOfficeId = Guid.NewGuid();
            var postOfficeResult = PostOffice.Create(postOfficeId, 1, "City1", "Address1", 20, 0, 0, postOfficeType);

            Assert.True(postOfficeResult.IsSuccess);
            var postOffice = postOfficeResult.Value;

            // Act
            await postOfficesRepository.CreateAsync(postOffice);

            // Assert
            var createdPostOffice = await postOfficesRepository.GetPostOfficeByIdAsync(postOfficeId);
            Assert.NotNull(createdPostOffice);
            Assert.Equal(postOffice.Id, createdPostOffice.Id);
            Assert.Equal(postOffice.City, createdPostOffice.City);
        }

        [Fact]
        public async Task CreateAsync_DuplicateCoordinates_ThrowsCoordinatesAlreadyExistException()
        {
            // Arrange
            var postOfficeType = await CreateTestPostOfficeTypeAsync();

            var postOfficeId = Guid.NewGuid();
            var postOffice = PostOffice.Create(postOfficeId, 1, "City1", "Address1", 20, 0, 0, postOfficeType).Value;

            await postOfficesRepository.CreateAsync(postOffice);

            // Act & Assert
            var duplicatePostOffice = PostOffice.Create(Guid.NewGuid(), 2, "City2", "Address1", 20, 0, 0, postOfficeType).Value;

            var exception = await Assert.ThrowsAsync<CoordinatesAlreadyExistException>(() =>
                postOfficesRepository.CreateAsync(duplicatePostOffice));

            Assert.Equal($"Post office with coordinates (latitude: {postOffice.Latitude}, " +
                $"longitude: {postOffice.Longitude}) already exist.", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_DuplicateNumberAddressCity_ThrowsNumberAddressCityAlreadyExistException()
        {
            // Arrange
            var postOfficeType = await CreateTestPostOfficeTypeAsync();

            var postOfficeId = Guid.NewGuid();
            var number = 1;
            var city = "City1";
            var address = "Address1";
            var postOffice = PostOffice.Create(postOfficeId, number, city, address, 20, 32.3, 56.2, postOfficeType).Value;

            await postOfficesRepository.CreateAsync(postOffice);

            // Act & Assert
            var duplicatePostOffice = PostOffice.Create(Guid.NewGuid(), number, city, address, 20, 0, 0, postOfficeType).Value;

            var exception = await Assert.ThrowsAsync<NumberAddressCityAlreadyExistException>(() =>
            postOfficesRepository.CreateAsync(duplicatePostOffice));

            Assert.Equal($"Post office with (Number: {number}, Address: {address}, City: {city}) already exists.", exception.Message);
        }
    }
}
