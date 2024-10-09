using Moq;

namespace Poshta.UnitTests.Services.PostOfficeServiceMethods
{
    public class DeleteAsyncTests
    {
        public class DeletePostOfficeTests : PostOfficeServiceTestsBase
        {
            [Fact]
            public async Task DeleteAsync_ShouldReturnSuccess_WhenPostOfficeDeleted()
            {
                // Arrange
                var postOfficeId = Guid.NewGuid();

                mockPostOfficesRepository.Setup(x => x.DeleteAsync(postOfficeId))
                    .Returns(Task.CompletedTask);

                // Act
                var result = await postOfficeService.DeleteAsync(postOfficeId);

                // Assert
                Assert.True(result.IsSuccess);
            }

            [Fact]
            public async Task DeleteAsync_ShouldLogError_WhenPostOfficeNotFound()
            {
                // Arrange
                var postOfficeId = Guid.NewGuid();

                mockPostOfficesRepository.Setup(x => x.DeleteAsync(postOfficeId))
                    .ThrowsAsync(new Exception("Post office not found"));

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => postOfficeService.DeleteAsync(postOfficeId));

                // Assert
                Assert.Equal("Post office not found", exception.Message);
            }
        }

    }
}
