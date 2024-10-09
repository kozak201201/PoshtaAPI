using Microsoft.EntityFrameworkCore;
using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories.PostOfficeTypeMethods
{
    public class CreateAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task CreateAsync_ValidPostOfficeType_CreatesSuccessfully()
        {
            // Arrange
            var postOfficeTypeId = Guid.NewGuid();
            var postOfficeType = PostOfficeType.Create(postOfficeTypeId, "TestType", 30f, 100f, 50f, 50f).Value;

            // Act
            await postOfficeTypesRepository.CreateAsync(postOfficeType);

            // Assert
            var createdPostOfficeType = await postOfficeTypesRepository.GetPostOfficeTypeByIdAsync(postOfficeTypeId);
            Assert.NotNull(createdPostOfficeType);
            Assert.Equal(postOfficeTypeId, createdPostOfficeType.Id);
            Assert.Equal("TestType", createdPostOfficeType.Name);
        }
    }
}
