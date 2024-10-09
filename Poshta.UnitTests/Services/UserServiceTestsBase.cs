using Microsoft.Extensions.Logging;
using Moq;
using Poshta.Application.Auth;
using Poshta.Application.Interfaces.Services;
using Poshta.Application.Services;
using Poshta.Core.Interfaces.Repositories;

namespace Poshta.UnitTests.Services
{
    public class UserServiceTestsBase
    {
        protected readonly Mock<IUsersRepository> usersRepository;
        protected readonly Mock<IJwtProvider> jwtProvider;
        protected readonly Mock<IConfirmationCodeService> confirmationCodeService;
        protected readonly Mock<IServiceProvider> serviceProvider;
        protected readonly Mock<ILogger<UserService>> logger;

        protected readonly UserService userService;

        public UserServiceTestsBase()
        {
            usersRepository = new Mock<IUsersRepository>();
            jwtProvider = new Mock<IJwtProvider>();
            confirmationCodeService = new Mock<IConfirmationCodeService>();
            serviceProvider = new Mock<IServiceProvider>();
            logger = new Mock<ILogger<UserService>>();

            userService = new UserService(
                usersRepository.Object,
                jwtProvider.Object,
                confirmationCodeService.Object,
                serviceProvider.Object,
                logger.Object
            );
        }
    }
}
