using Microsoft.Extensions.Logging;
using Moq;
using Poshta.Application.Interfaces.Services;
using Poshta.Application.Services;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Interfaces.Services;

namespace Poshta.UnitTests.Services
{
    public class ShipmentServiceTestsBase
    {
        protected readonly Mock<IShipmentsRepository> shipmentsRepositoryMock;
        protected readonly Mock<IUserService> userServiceMock;
        protected readonly Mock<IOperatorService> operatorServiceMock;
        protected readonly Mock<IPostOfficeService> postOfficeServiceMock;
        protected readonly Mock<INotificationService> notificationServiceMock;
        protected readonly Mock<ILogger<ShipmentService>> loggerMock;
        protected readonly ShipmentService shipmentService;

        public ShipmentServiceTestsBase()
        {
            shipmentsRepositoryMock = new Mock<IShipmentsRepository>();
            userServiceMock = new Mock<IUserService>();
            operatorServiceMock = new Mock<IOperatorService>();
            postOfficeServiceMock = new Mock<IPostOfficeService>();
            notificationServiceMock = new Mock<INotificationService>();
            loggerMock = new Mock<ILogger<ShipmentService>>();

            shipmentService = new ShipmentService(
                shipmentsRepositoryMock.Object,
                userServiceMock.Object,
                operatorServiceMock.Object,
                postOfficeServiceMock.Object,
                notificationServiceMock.Object,
                loggerMock.Object
            );
        }
    }
}
