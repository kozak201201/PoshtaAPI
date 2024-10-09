using Microsoft.Extensions.Logging;
using Moq;
using Poshta.Application.Services;
using Poshta.Core.Interfaces.Repositories;

namespace Poshta.UnitTests.Services
{
    public class PostOfficeTypeServiceTestsBase
    {
        protected readonly Mock<IPostOfficeTypesRepository> postOfficeTypesRepository;
        protected readonly Mock<ILogger<PostOfficeTypeService>> logger;
        protected readonly PostOfficeTypeService postOfficeTypeService;

        public PostOfficeTypeServiceTestsBase()
        {
            postOfficeTypesRepository = new Mock<IPostOfficeTypesRepository>();
            logger = new Mock<ILogger<PostOfficeTypeService>>();
            postOfficeTypeService = new PostOfficeTypeService(postOfficeTypesRepository.Object, logger.Object);
        }
    }
}
