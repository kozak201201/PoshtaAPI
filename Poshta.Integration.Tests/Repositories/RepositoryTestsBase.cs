using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Poshta.DataAccess.SqlServer.Entities;
using Poshta.DataAccess.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Poshta.DataAccess.SqlServer.Repositories;
using Poshta.DataAccess.SqlServer.Mappings;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Models;

namespace Poshta.IntegrationTests.Repositories
{
    public abstract class RepositoryTestsBase : IDisposable
    {
        protected readonly UserManager<UserEntity> userManager;
        protected readonly ApplicationDbContext context;
        protected readonly IMapper mapper;
        protected readonly IUsersRepository usersRepository;
        protected readonly IPostOfficeTypesRepository postOfficeTypesRepository;
        protected readonly IPostOfficesRepository postOfficesRepository;
        protected readonly IShipmentsRepository shipmentsRepository;
        protected readonly IOperatorsRepository operatorsRepository;

        protected RepositoryTestsBase()
        {
            var services = new ServiceCollection();

            services.AddAutoMapper(typeof(DataBaseMappings));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("DataSource=:memory:"));

            services.AddIdentityCore<UserEntity>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = false;
            })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var serviceProvider = services.BuildServiceProvider();

            mapper = serviceProvider.GetRequiredService<IMapper>();
            userManager = serviceProvider.GetRequiredService<UserManager<UserEntity>>();
            context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            usersRepository = new UsersRepository(context, userManager, mapper);
            postOfficeTypesRepository = new PostOfficeTypesRepository(context, mapper);
            postOfficesRepository = new PostOfficesRepository(context, mapper);
            shipmentsRepository = new ShipmentsRepository(context, mapper);
            operatorsRepository = new OperatorsRepository(context, userManager, mapper);

            context.Database.OpenConnection();
            context.Database.EnsureCreated();
        }

        public async Task<PostOfficeType> CreateTestPostOfficeTypeAsync()
        {
            var id = Guid.NewGuid();
            var postOfficeTypeResult = PostOfficeType.Create(id, "PostOfficeUpTo60Kg", 60f, 100f, 50f, 50f);

            Assert.True(postOfficeTypeResult.IsSuccess);
            var postOfficeType = postOfficeTypeResult.Value;

            await postOfficeTypesRepository.CreateAsync(postOfficeType);

            return postOfficeType;
        }
        
        public async Task<PostOffice> CreateTestPostOffice1Async()
        {
            //Arrange
            var id = Guid.NewGuid();
            var postOfficeTypeResult = PostOfficeType.Create(id, "PostOfficeUpTo15Kg", 15f, 100f, 50f, 50f);

            Assert.True(postOfficeTypeResult.IsSuccess);
            var postOfficeType = postOfficeTypeResult.Value;

            await postOfficeTypesRepository.CreateAsync(postOfficeType);

            var postOffice1Id = Guid.NewGuid();

            var postOffice1Result = PostOffice.Create(
                postOffice1Id,
                1,
                "City1",
                "Address1",
                20,
                0,
                0,
                postOfficeType);

            Assert.True(postOffice1Result.IsSuccess);

            var postOffice1 = postOffice1Result.Value;

            //Act
            await postOfficesRepository.CreateAsync(postOffice1);

            //Assert
            var createdPostOffice = await postOfficesRepository.GetPostOfficeByIdAsync(postOffice1Id);

            Assert.NotNull(createdPostOffice);
            Assert.Equal(postOffice1.Id, createdPostOffice.Id);

            return postOffice1;
        }

        public async Task<PostOffice> CreateTestPostOffice2Async()
        {
            //Arrange
            var id = Guid.NewGuid();
            var postOfficeTypeResult = PostOfficeType.Create(id, "PostOfficeUpTo30Kg", 30f, 100f, 50f, 50f);

            Assert.True(postOfficeTypeResult.IsSuccess);
            var postOfficeType = postOfficeTypeResult.Value;

            await postOfficeTypesRepository.CreateAsync(postOfficeType);

            var postOfficeId2 = Guid.NewGuid();

            var postOffice2Result = PostOffice.Create(
                postOfficeId2,
                2,
                "City2",
                "Address2",
                20,
                20,
                10,
                postOfficeType);

            Assert.True(postOffice2Result.IsSuccess);

            var postOffice2 = postOffice2Result.Value;

            //Act
            await postOfficesRepository.CreateAsync(postOffice2);

            //Assert
            var createdPostOffice = await postOfficesRepository.GetPostOfficeByIdAsync(postOfficeId2);

            Assert.NotNull(createdPostOffice);
            Assert.Equal(postOffice2.Id, createdPostOffice.Id);

            return postOffice2;
        }

        public async Task<User> CreateTestUser1Async()
        {
            //Arrange
            var senderId = Guid.NewGuid();
            var senderResult = User.Create(senderId, "SenderLastName", "SenderFirstName", "password", "+1234567891");

            Assert.True(senderResult.IsSuccess);

            var sender = senderResult.Value;

            //Act
            await usersRepository.CreateAsync(sender);

            //Assert
            var createdUser = await usersRepository.GetByIdAsync(senderId);

            Assert.NotNull(createdUser);
            Assert.Equal(sender.Id, createdUser.Id);

            return sender;
        }

        public async Task<User> CreateTestUser2Async()
        {
            //Arrange
            var recipientId = Guid.NewGuid();

            var recipientResult = User.Create(recipientId, "RecipientLastName", "RecipientFirstName", "password", "+1234567892");

            Assert.True(recipientResult.IsSuccess);

            var recipient = recipientResult.Value;

            //Act
            await usersRepository.CreateAsync(recipient);

            //Assert
            var createdUser = await usersRepository.GetByIdAsync(recipientId);

            Assert.NotNull(createdUser);
            Assert.Equal(recipient.Id, createdUser.Id);

            return recipient;
        }

        public void Dispose()
        {
            context.Database.CloseConnection();
            context.Dispose();
        }
    }
}