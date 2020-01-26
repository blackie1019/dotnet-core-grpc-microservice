#region

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockSite.Core.Factories;
using MockSite.Core.Interfaces;
using MockSite.Core.Repositories;
using MockSite.DomainService;
using MockSite.Message;
using NSubstitute;
using NUnit.Framework;
using UserService = MockSite.Core.Services.UserService;
using Bogus;
using MockSite.Common.Core.Utilities;
using MockSite.Common.Data.Utilities;
using MockSite.Core.Entities;
using MockSite.DomainService.Utilities;
using NSubstitute.Core;

#endregion

namespace MockSite.Test
{
    [Parallelizable]
    [TestFixture]
    public class UserTest
    {
        private IConfiguration _config;
        private ILogger<UserRepository> _logger;
        private SqlConnectionHelper _helper;

        [OneTimeSetUp]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger<UserRepository>>();

            var consulProvider =
                ConsulConfigProvider.LoadConsulConfig("http://127.0.0.1:18500/v1/kv/",
                    new[] {"MockSite"});
            _config = new ConfigurationBuilder()
                .AddJsonFile(consulProvider, "test.json", true, false)
                .Build();

            _helper = new SqlConnectionHelper(Substitute.For<ILoggerProvider>());
            MethodTimeLogger.SetLogger(Substitute.For<ILoggerProvider>());
        }

        private UserServiceImpl GetServiceImpl(IUserRepository fakeUserRepository = null,
            IRedisUserRepository fakeRedisRepo = null)
        {
            var redisRepo = fakeRedisRepo ?? new RedisUserRepository(_config, new RedisConnectHelper());
            var userRepo = fakeUserRepository ?? new UserRepository(_logger, _config, _helper, redisRepo);

            return new UserServiceImpl(
                new UserService(
                    _config,
                    new UserRepositoryFactory(
                        userRepo,
                        Substitute.For<IMongoUserRepository>())
                ));
        }

        [Test]
        public async Task F01_00_Create_User()
        {
            // Arrange
            var createMessage = CreateUserMessage();

            // Action
            var serviceImpl = GetServiceImpl();
            var actualResponse = await serviceImpl.Create(createMessage, null);
            var actualUserResponse = await serviceImpl.Get(new QueryUserMessage {Id = actualResponse.Id}, null);

            // Assert
            Assert.AreEqual(createMessage.Code, actualUserResponse.Data.Code);
            Assert.AreEqual(createMessage.Name, actualUserResponse.Data.Name);
            Assert.AreEqual(createMessage.Email, actualUserResponse.Data.Email);
            Assert.IsEmpty(actualUserResponse.Message);
        }

        [Test]
        public async Task F01_01_Create_User_Db_Failed()
        {
            // Arrange
            var createMessage = CreateUserMessage();
            var fakeRedisRepo = Substitute.For<IRedisUserRepository>();

            // Action
            var serviceImpl = GetServiceImpl(fakeRedisRepo: fakeRedisRepo);
            await serviceImpl.Create(createMessage, null);
            var actualResponse = (await serviceImpl.Create(createMessage, null));

            // Assert
            Assert.IsNull(actualResponse);
            await fakeRedisRepo.Received(1).Create(Arg.Is<UserEntity>(u => u.Code == createMessage.Code));
        }

        [Test]
        public async Task F01_02_Create_User_Redis_Failed()
        {
            // Arrange
            var createMessage = CreateUserMessage();
            var queryUsersMessage = new QueryUsersMessage {Code = createMessage.Code};
            var fakeRedisRepo = Substitute.For<IRedisUserRepository>();
            fakeRedisRepo.Create(null).ReturnsForAnyArgs((Func<CallInfo, int>) (c => throw new Exception()));

            // Action
            var serviceImpl = GetServiceImpl(fakeRedisRepo: fakeRedisRepo);
            var actualResponse = await serviceImpl.Create(createMessage, null);
            var allUsers = (await serviceImpl.GetAll(queryUsersMessage, null)).Data;
            var actualUser = allUsers.FirstOrDefault(u => u.Code == createMessage.Code);


            // Assert;
            Assert.IsNull(actualResponse);
            Assert.IsNull(actualUser);
            await fakeRedisRepo.Received(1).Create(Arg.Is<UserEntity>(u => u.Code == createMessage.Code));
        }

        [Test]
        public async Task F02_Get_AllUsers()
        {
            // Action
            var actualResult = await GetServiceImpl().GetAll(new QueryUsersMessage(), null);

            // Assert
            Assert.IsNotEmpty(actualResult.Data);
        }

        [Test]
        public async Task F03_00_Get_User_By_Name()
        {
            // Arrange
            var createMessage = CreateUserMessage();
            var queryUsersMessage = new QueryUsersMessage {Name = createMessage.Name};

            // Action
            var serviceImpl = GetServiceImpl();
            await serviceImpl.Create(createMessage, null);
            var allUsers = (await serviceImpl.GetAll(queryUsersMessage, null)).Data;
            var actualUser = allUsers.First();

            // Assert
            Assert.AreEqual(createMessage.Code, actualUser.Code);
            Assert.AreEqual(createMessage.Name, actualUser.Name);
            Assert.AreEqual(createMessage.Email, actualUser.Email);
        }

        [Ignore("Not Implemented")]
        [Test]
        public async Task F03_01_Get_User_By_Name_Keyword()
        {
            //Arrange
            var createMessage = CreateUserMessage();
            var truncateName = createMessage.Name.Substring(0, 2);
            var queryUsersMessage = new QueryUsersMessage {Name = truncateName};

            // Action
            var serviceImpl = GetServiceImpl();
            var actualResponseCode = (await serviceImpl.Create(createMessage, null)).Code;
            var allUsers = (await serviceImpl.GetAll(queryUsersMessage, null)).Data;
            var actualUser = allUsers.First();

            // Assert
            Assert.AreEqual(ResponseCode.Success, actualResponseCode);
            Assert.AreEqual(createMessage.Code, actualUser.Code);
            Assert.AreEqual(createMessage.Name, actualUser.Name);
            Assert.AreEqual(createMessage.Email, actualUser.Email);
        }

        [Test]
        public async Task F03_02_Get_User_By_Email()
        {
            var createMessage = CreateUserMessage();
            var queryUsersMessage = new QueryUsersMessage {Email = createMessage.Email};

            // Action
            var serviceImpl = GetServiceImpl();
            await serviceImpl.Create(createMessage, null);
            var allUsers = (await serviceImpl.GetAll(queryUsersMessage, null)).Data;
            var actualUser = allUsers.First();

            // Assert
            Assert.AreEqual(createMessage.Code, actualUser.Code);
            Assert.AreEqual(createMessage.Name, actualUser.Name);
            Assert.AreEqual(createMessage.Email, actualUser.Email);
        }

        [Test]
        public async Task F04_Authenticate()
        {
            //Arrange
            var createMessage = CreateUserMessage();

            // Action
            var serviceImpl = GetServiceImpl();
            await serviceImpl.Create(createMessage, null);

            var actualResponse = await serviceImpl
                .Authenticate(new AuthenticateMessage {Name = createMessage.Name, Password = createMessage.Password},
                    null);

            var authenticateResponseCode = actualResponse.Code;

            var actualUser = actualResponse.Data;

            // Assert
            Assert.AreEqual(ResponseCode.Success, authenticateResponseCode);
            Assert.IsNotNull(actualUser);
            Assert.AreEqual(createMessage.Code, actualUser.Code);
        }

        [Test]
        public async Task F05_00_Update_User()
        {
            // Arrange
            var serviceImpl = GetServiceImpl();
            var createMessage = CreateUserMessage();
            var createdUser = await serviceImpl.Create(createMessage, null);
            var newEmail = createdUser.Email + ".test";
            var newName = createdUser.Name + ".test";
            var updateMessage = new UpdateUserMessage
            {
                Id = createdUser.Id,
                Email = newEmail,
                Name = newName
            };

            // Action
            var actualResponseCode = (await serviceImpl.Update(updateMessage, null)).Code;
            var actualUserResponse = await serviceImpl.Get(new QueryUserMessage {Id = createdUser.Id}, null);

            // Assert
            Assert.AreEqual(ResponseCode.Success, actualResponseCode);
            Assert.AreEqual(newName, actualUserResponse.Data.Name);
            Assert.AreEqual(newEmail, actualUserResponse.Data.Email);
            Assert.IsEmpty(actualUserResponse.Message);
        }

        [Test]
        public async Task F05_01_Update_User_Db_failed()
        {
            // Arrange
            var fakeRedisRepo = Substitute.For<IRedisUserRepository>();
            var serviceImpl = GetServiceImpl(fakeRedisRepo: fakeRedisRepo);
            var createMessage = CreateUserMessage();
            var createdUser = await serviceImpl.Create(createMessage, null);
            var newEmail = createdUser.Email + ".test";
            var newName = "12345678901234567890123456789012345678901234567890"; // over the field size 40 chars
            var updateMessage = new UpdateUserMessage
            {
                Id = createdUser.Id,
                Email = newEmail,
                Name = newName
            };

            // Action
            var actualResponseCode = (await serviceImpl.Update(updateMessage, null)).Code;
            var actualUserResponse = await serviceImpl.Get(new QueryUserMessage {Id = createdUser.Id}, null);

            // Assert
            Assert.AreEqual(ResponseCode.GeneralError, actualResponseCode);
            Assert.AreEqual(createMessage.Name, actualUserResponse.Data.Name);
            Assert.AreEqual(createMessage.Email, actualUserResponse.Data.Email);
            Assert.IsEmpty(actualUserResponse.Message);
            await fakeRedisRepo.DidNotReceiveWithAnyArgs().Update(null);
        }

        [Test]
        public async Task F05_02_Update_User_Redis_failed()
        {
            // Arrange
            var fakeRedisRepo = Substitute.For<IRedisUserRepository>();
            fakeRedisRepo.Update(null).ReturnsForAnyArgs(_ => throw new Exception());
            var serviceImpl = GetServiceImpl(fakeRedisRepo: fakeRedisRepo);
            var createMessage = CreateUserMessage();
            var createdUser = await serviceImpl.Create(createMessage, null);
            var newEmail = createdUser.Email + ".test";
            var newName = createdUser.Name + ".test";
            var updateMessage = new UpdateUserMessage
            {
                Id = createdUser.Id,
                Email = newEmail,
                Name = newName
            };

            // Action
            var actualResponseCode = (await serviceImpl.Update(updateMessage, null)).Code;
            var actualUserResponse = await serviceImpl.Get(new QueryUserMessage {Id = createdUser.Id}, null);

            // Assert
            Assert.AreEqual(ResponseCode.GeneralError, actualResponseCode);
            Assert.AreEqual(createMessage.Name, actualUserResponse.Data.Name);
            Assert.AreEqual(createMessage.Email, actualUserResponse.Data.Email);
            Assert.IsEmpty(actualUserResponse.Message);
            await fakeRedisRepo.ReceivedWithAnyArgs(1).Update(null);
        }

        [Test]
        public async Task F06_00_Delete_User()
        {
            // Arrange
            var createMessage = CreateUserMessage();
            var serviceImpl = GetServiceImpl();
            var createdUser = await serviceImpl.Create(createMessage, null);
            var deleteMessage = new QueryUserMessage {Id = createdUser.Id};

            // Action
            var actualResponseCode = (await serviceImpl.Delete(deleteMessage, null)).Code;
            var actualUserResponse = await GetServiceImpl().Get(new QueryUserMessage{Id = deleteMessage.Id}, null);

            // Assert
            Assert.AreEqual(ResponseCode.Success, actualResponseCode);
            Assert.AreEqual(ResponseCode.NotFound, actualUserResponse.Code);
            Assert.IsNull(actualUserResponse.Data);
            Assert.IsEmpty(actualUserResponse.Message);
        }

        [Test]
        public async Task F06_01_Delete_User_Db_failed()
        {
            // Arrange
            var deleteMessage = new QueryUserMessage {Id = 1};
            var fakeUserRepo = Substitute.For<IUserRepository>();
            fakeUserRepo.Delete(Arg.Is<int>(id => id == deleteMessage.Id)).Returns(_ => throw new Exception());
            var fakeRedisRepo = Substitute.For<IRedisUserRepository>();
            var serviceImpl = GetServiceImpl(fakeUserRepo, fakeRedisRepo);

            // Action
            var actualResponseCode = (await serviceImpl.Delete(deleteMessage, null)).Code;
            var actualUserResponse = await GetServiceImpl().Get(new QueryUserMessage{Id = deleteMessage.Id}, null);

            // Assert
            Assert.AreEqual(ResponseCode.GeneralError, actualResponseCode);
            Assert.IsNotNull(actualUserResponse.Data);
            Assert.IsEmpty(actualUserResponse.Message);
            await fakeRedisRepo.DidNotReceiveWithAnyArgs().Delete(1);
        }

        [Test]
        public async Task F06_02_Delete_User_Redis_failed()
        {
            // Arrange
            var deleteMessage = new QueryUserMessage {Id = 1};
            var fakeRedisRepo = Substitute.For<IRedisUserRepository>();
            fakeRedisRepo.Delete(1).ReturnsForAnyArgs(_ => throw new Exception());
            var serviceImpl = GetServiceImpl(fakeRedisRepo: fakeRedisRepo);

            // Action
            var actualResponseCode = (await serviceImpl.Delete(deleteMessage, null)).Code;
            var actualUserResponse = await GetServiceImpl().Get(new QueryUserMessage{Id = deleteMessage.Id}, null);

            // Assert
            Assert.AreEqual(ResponseCode.GeneralError, actualResponseCode);
            Assert.IsNotNull(actualUserResponse.Data);
            Assert.IsEmpty(actualUserResponse.Message);
            await fakeRedisRepo.Received(1).Delete(1);
        }

        private static CreateUserMessage CreateUserMessage()
        {
            const string password = "pass.123";
            var testCode = Guid.NewGuid().ToString("N").Substring(0, 8);
            var testEmail = new Faker().Internet.Email();
            var testName = new Faker().Name.FullName();
            var createMessage = new CreateUserMessage
            {
                Code = testCode,
                Email = testEmail,
                Name = testName,
                Password = password
            };

            return createMessage;
        }
    }
}