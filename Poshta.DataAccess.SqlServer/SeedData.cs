using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Models;
using System.Transactions;

namespace Poshta.DataAccess.SqlServer
{
    public class SeedData(
        IUsersRepository usersRepository,
        IPostOfficeTypesRepository postOfficeTypesRepository,
        IPostOfficesRepository postOfficesRepository)
    {
        private readonly IUsersRepository usersRepository = usersRepository;
        private readonly IPostOfficeTypesRepository postOfficeTypesRepository = postOfficeTypesRepository;
        private readonly IPostOfficesRepository postOfficesRepository = postOfficesRepository;

        public async Task SeedDataAsync()
        {
            await SeedUsersAsync();
            var postOfficeTypes = await SeedPostOfficeTypesAsync();
            await SeedPostOfficesAsync(postOfficeTypes);
        }

        private async Task SeedUsersAsync()
        {
            var testAdminPhoneNumber = "+1234567890";
            var testAdminGuidId = new Guid("00000000-0000-0000-0000-000000000001");

            var adminUserResult = User.Create(
                testAdminGuidId,
                "TestUserLastName",
                "TestUserFirstName",
                "adminPassword", //password
                testAdminPhoneNumber);

            if (adminUserResult.IsFailure)
            {
                throw new Exception($"Seed data exception. Can't create test admin user. {adminUserResult.Error}");
            }

            var adminUser = adminUserResult.Value;

            var getAdminFromDb = await usersRepository.GetByPhoneAsync(testAdminPhoneNumber);

            //already created
            if (getAdminFromDb != null) return;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await usersRepository.CreateAsync(adminUser);

                if (result.IsFailure)
                {
                    throw new Exception($"Seed data exception. User wasn't created. {result.Error}");
                }

                var addRoleResult = await usersRepository.AddRoleAsync(adminUser.Id, "Admin");

                if (addRoleResult.IsFailure)
                {
                    throw new Exception($"Seed data exception. Can't add role to admin user. {addRoleResult.Error}");
                }

                transaction.Complete();
            }

            //create test user models

            var testUser1GuidId = new Guid("00000000-0000-0000-0000-000000000010");
            var testUser2GuidId = new Guid("00000000-0000-0000-0000-000000000020");
            var testUser3GuidId = new Guid("00000000-0000-0000-0000-000000000030");
            var testUser4GuidId = new Guid("00000000-0000-0000-0000-000000000040");


            var testUser1Result = User.Create(
                testUser1GuidId,
                "TestUserLastName",
                "TestUserFirstName",
                "userPassword", //password
                "+1234567891");

            if (testUser1Result.IsFailure)
            {
                throw new Exception($"Seed data exception. Can't create test user. {testUser1Result.Error}");
            }

            var testUser1 = testUser1Result.Value;

            var testUser2Result = User.Create(
                testUser2GuidId,
                "TestUserLastName",
                "TestUserFirstName",
                "userPassword", //password
                "+1234567892");

            if (testUser2Result.IsFailure)
            {
                throw new Exception($"Seed data exception. Can't create test user. {testUser2Result.Error}");
            }

            var testUser2 = testUser2Result.Value;

            var testUser3Result = User.Create(
                testUser3GuidId,
                "TestUserLastName",
                "TestUserFirstName",
                "userPassword", //password
                "+1234567893");

            if (testUser3Result.IsFailure)
            {
                throw new Exception($"Seed data exception. Can't create test user. {testUser3Result.Error}");
            }

            var testUser3 = testUser3Result.Value;

            var testUser4Result = User.Create(
                testUser4GuidId,
                "TestUserLastName",
                "TestUserFirstName",
                "userPassword", //password
                "+1234567894");

            if (testUser4Result.IsFailure)
            {
                throw new Exception($"Seed data exception. Can't create test user. {testUser4Result.Error}");
            }

            var testUser4 = testUser4Result.Value;

            //write user models in db

            var createUser1Result = await usersRepository.CreateAsync(testUser1);

            if (createUser1Result.IsFailure)
            {
                throw new Exception($"Seed data exception. User wasn't created. {createUser1Result.Error}");
            }

            var createUser2Result = await usersRepository.CreateAsync(testUser2);

            if (createUser2Result.IsFailure)
            {
                throw new Exception($"Seed data exception. User wasn't created. {createUser2Result.Error}");
            }

            var createUser3Result = await usersRepository.CreateAsync(testUser3);

            if (createUser3Result.IsFailure)
            {
                throw new Exception($"Seed data exception. User wasn't created. {createUser3Result.Error}");
            }

            var createUser4Result = await usersRepository.CreateAsync(testUser4);

            if (createUser4Result.IsFailure)
            {
                throw new Exception($"Seed data exception. User wasn't created. {createUser4Result.Error}");
            }
        }

        private async Task<List<PostOfficeType>> SeedPostOfficeTypesAsync()
        {
            var testPostOfficeTypeId = new Guid("00000000-0000-0000-0000-000000000100");

            var testPostOfficeTypeFromDb = await postOfficeTypesRepository.GetPostOfficeTypeByIdAsync(testPostOfficeTypeId);

            if (testPostOfficeTypeFromDb != null) return [];

            var testPostOfficeTypeResult = PostOfficeType.Create(
                testPostOfficeTypeId,
                "TestType",
                30,
                30,
                30,
                30);

            var postOfficeType1Result = PostOfficeType.Create(
                Guid.NewGuid(),
                "Poshtomat",
                20,
                40,
                60,
                30);

            var postOfficeType2Result = PostOfficeType.Create(
                Guid.NewGuid(),
                "PostOfficeUpTo5",
                5,
                60,
                60,
                60);

            var postOfficeType3Result = PostOfficeType.Create(
                Guid.NewGuid(),
                "PostOfficeUpTo30Kg",
                30,
                120,
                70,
                70);

            var postOfficeType4Result = PostOfficeType.Create(
                Guid.NewGuid(),
                "PostOfficeUpTo1000Kg",
                1000,
                300,
                300,
                170);

            if (testPostOfficeTypeResult.IsFailure)
                throw new Exception($"Seed data exception. Can't add post office type. {testPostOfficeTypeResult.Error}");

            if (postOfficeType1Result.IsFailure)
                throw new Exception($"Seed data exception. Can't add post office type. {postOfficeType1Result.Error}");

            if (postOfficeType2Result.IsFailure)
                throw new Exception($"Seed data exception. Can't add post office type. {postOfficeType2Result.Error}");

            if (postOfficeType3Result.IsFailure)
                throw new Exception($"Seed data exception. Can't add post office type. {postOfficeType3Result.Error}");

            if (postOfficeType4Result.IsFailure)
                throw new Exception($"Seed data exception. Can't add post office type. {postOfficeType4Result.Error}");

            await postOfficeTypesRepository.CreateAsync(testPostOfficeTypeResult.Value);
            await postOfficeTypesRepository.CreateAsync(postOfficeType1Result.Value);
            await postOfficeTypesRepository.CreateAsync(postOfficeType2Result.Value);
            await postOfficeTypesRepository.CreateAsync(postOfficeType3Result.Value);
            await postOfficeTypesRepository.CreateAsync(postOfficeType4Result.Value);

            var postOfficeTypes = new List<PostOfficeType>()
            {
                testPostOfficeTypeResult.Value,
                postOfficeType1Result.Value,
                postOfficeType2Result.Value,
                postOfficeType3Result.Value,
                postOfficeType4Result.Value,
            };

            return postOfficeTypes;
        }

        private async Task SeedPostOfficesAsync(List<PostOfficeType> postOfficeTypes)
        {
            //already seed
            if (postOfficeTypes.Count == 0) return;

            var defaultPostOfficeType = postOfficeTypes.FirstOrDefault(poType => poType.Name == "PostOfficeUpTo30Kg");

            if (defaultPostOfficeType == null)
            {
                throw new Exception("Seed data exception. Can't find default post office type to create post offices.");
            }

            var postOffice1Id = new Guid("00000000-0000-0000-0000-000000001000");

            var postOffice1Result = PostOffice.Create(
                postOffice1Id,
                1, // if you create second post office in kyiv again write number 2. Business logic
                "Kyiv",
                "Street 1, Kyiv",
                100,
                50.4501, // latitude
                30.5234, // longitude
                defaultPostOfficeType // may be different
            );

            var postOffice2Result = PostOffice.Create(
                Guid.NewGuid(),
                1, // first in lviv
                "Lviv",
                "Street 2, Lviv",
                150,
                49.8397,
                24.0297,
                defaultPostOfficeType
            );

            var postOffice3Result = PostOffice.Create(
                Guid.NewGuid(),
                1,
                "Odesa",
                "Street 3, Odesa",
                200,
                46.4825,
                30.7233,
                defaultPostOfficeType
            );

            var postOffice4Result = PostOffice.Create(
                Guid.NewGuid(),
                1,
                "Kharkiv",
                "Street 4, Kharkiv",
                250,
                49.9935,
                36.2304,
                defaultPostOfficeType
            );

            if (postOffice1Result.IsFailure)
                throw new Exception($"Seed data exception. Can't add post office. {postOffice1Result.Error}");

            if (postOffice2Result.IsFailure)
                throw new Exception($"Seed data exception. Can't add post office. {postOffice2Result.Error}");

            if (postOffice3Result.IsFailure)
                throw new Exception($"Seed data exception. Can't add post office. {postOffice3Result.Error}");

            if (postOffice4Result.IsFailure)
                throw new Exception($"Seed data exception. Can't add post office. {postOffice4Result.Error}");

            var postOffice1FromDb = await postOfficesRepository.GetPostOfficeByIdAsync(postOffice1Id);

            if (postOffice1FromDb != null) return;

            await postOfficesRepository.CreateAsync(postOffice1Result.Value);
            await postOfficesRepository.CreateAsync(postOffice2Result.Value);
            await postOfficesRepository.CreateAsync(postOffice3Result.Value);
            await postOfficesRepository.CreateAsync(postOffice4Result.Value);
        }
    }
}
