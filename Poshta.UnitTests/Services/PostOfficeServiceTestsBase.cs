using Microsoft.Extensions.Logging;
using Moq;
using Poshta.Application.Services;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;

namespace Poshta.UnitTests.Services
{
    public class PostOfficeServiceTestsBase
    {
        protected readonly Mock<IPostOfficeTypeService> mockPostOfficeTypeService;
        protected readonly Mock<IPostOfficesRepository> mockPostOfficesRepository;
        protected readonly Mock<ILogger<PostOfficeService>> mockLogger;
        protected readonly PostOfficeService postOfficeService;

        public PostOfficeServiceTestsBase()
        {
            mockPostOfficeTypeService = new Mock<IPostOfficeTypeService>();
            mockPostOfficesRepository = new Mock<IPostOfficesRepository>();
            mockLogger = new Mock<ILogger<PostOfficeService>>();
            postOfficeService = new PostOfficeService(
                mockPostOfficeTypeService.Object,
                mockPostOfficesRepository.Object,
                mockLogger.Object
            );
        }

        public PostOfficeType CreateDefaultPostOfficeType()
        {
            var postOfficeTypeResult = PostOfficeType.Create(
                Guid.NewGuid(),
                "TestPostOfficeType",
                30f,
                15f,
                15f,
                15f);

            Assert.True(postOfficeTypeResult.IsSuccess);

            var postOffice = postOfficeTypeResult.Value;

            return postOffice;
        }
    }
}
