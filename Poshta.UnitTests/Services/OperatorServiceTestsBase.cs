using Microsoft.Extensions.Logging;
using Moq;
using Poshta.Application.Services;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Interfaces.Services;

namespace Poshta.UnitTests.Services
{
    public class OperatorServiceTestsBase
    {
        protected readonly Mock<IOperatorsRepository> mockOperatorsRepository;
        protected readonly Mock<IUserService> mockUserService;
        protected readonly Mock<ILogger<OperatorService>> mockLogger;
        protected readonly Mock<IServiceProvider> mockServiceProvider;
        protected readonly OperatorService operatorService;

        public OperatorServiceTestsBase()
        {
            mockOperatorsRepository = new Mock<IOperatorsRepository>();
            mockUserService = new Mock<IUserService>();
            mockLogger = new Mock<ILogger<OperatorService>>();
            mockServiceProvider = new Mock<IServiceProvider>();
            operatorService = new OperatorService(
                mockOperatorsRepository.Object,
                mockUserService.Object,
                mockServiceProvider.Object,
                mockLogger.Object
            );
        }
    }
}