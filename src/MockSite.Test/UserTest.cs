#region

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Core.Repositories;
using MockSite.DomainService;
using MockSite.Message;
using MySql.Data.MySqlClient;
using NSubstitute;
using NUnit.Framework;

#endregion

namespace MockSite.Test
{
    [Parallelizable]
    [TestFixture]
    public class UserTest
    {
        private string _connectionString;
        private IConfiguration _config;
        private ILogger<UserRepository> _logger;

        [OneTimeSetUp]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger<UserRepository>>();
            _config = Substitute.For<IConfiguration>();
            _connectionString = ConsulSettingHelper.Instance.GetValueFromKey(DbConnectionConst.TestKey);
            _config.GetSection(DbConnectionConst.TestKey).Value.Returns(_connectionString);
            var useDb = ConsulSettingHelper.Instance.GetValueFromKey(DbConnectionConst.UseDbKey);
            _config.GetSection(DbConnectionConst.UseDbKey).Value.Returns(useDb);
        }

        [Test]
        public async Task Test_Create_User()
        {
            // Arrange
            const string code = "testNew001";
            const string email = "TestNew001@gmail.com";
            const string name = "TestNew001";
            const string password = "pass.123";
            var createMessage = new CreateUserMessage
            {
                Code = code,
                Email = email,
                Name = name,
                Password = password
            };
            var expectedUserCount = default(int);

            // Action
            var actualResponseCode = default(ResponseCode);
            var actualUser = default(User);
            var actualUserCount = default(int);
            await RollBackTransactionScope(async (connection, transaction) =>
            {
                var serviceImpl = GetServiceImpl(connection, transaction);
                expectedUserCount = (await serviceImpl.GetAll(new Empty(), null)).Value.Count + 1;
                actualResponseCode = (await serviceImpl.Create(createMessage, null)).Code;
                var allUsers = (await serviceImpl.GetAll(new Empty(), null)).Value;
                actualUser = allUsers.First(user => user.Code == code);
                actualUserCount = allUsers.Count;
            });

            // Assert
            Assert.AreEqual(ResponseCode.Success, actualResponseCode);
            Assert.AreEqual(code, actualUser.Code);
            Assert.AreEqual(name, actualUser.Name);
            Assert.AreEqual(email, actualUser.Email);
            Assert.AreEqual(expectedUserCount, actualUserCount);
        }

        [Test]
        public async Task Test_Update_User()
        {
            // Arrange
            var targetUser = (await GetServiceImpl().GetAll(new Empty(), null)).Value.First();
            var newEmail = targetUser.Email + ".test";
            var newName = targetUser.Name + ".test";
            var updateMessage = new UpdateUserMessage
            {
                Id = targetUser.Id,
                Email = newEmail,
                Name = newName
            };

            // Action
            var actualResponseCode = default(ResponseCode);
            var actualUser = default(User);
            await RollBackTransactionScope(async (connection, transaction) =>
            {
                var serviceImpl = GetServiceImpl(connection, transaction);
                actualResponseCode = (await serviceImpl.Update(updateMessage, null)).Code;
                actualUser = await serviceImpl.Get(new QueryUserMessage {Id = targetUser.Id}, null);
            });

            // Assert
            Assert.AreEqual(ResponseCode.Success, actualResponseCode);
            Assert.AreEqual(newName, actualUser.Name);
            Assert.AreEqual(newEmail, actualUser.Email);
        }

        [Test]
        public async Task Test_Delete_User()
        {
            // Arrange
            var targetUser = (await GetServiceImpl().GetAll(new Empty(), null)).Value.First();
            var deleteMessage = new QueryUserMessage {Id = targetUser.Id};
            var expectedUserCount = default(int);

            // Action
            var actualResponseCode = default(ResponseCode);
            var actualUser = default(User);
            var actualUserCount = default(int);
            await RollBackTransactionScope(async (connection, transaction) =>
            {
                var serviceImpl = GetServiceImpl(connection, transaction);
                expectedUserCount = (await serviceImpl.GetAll(new Empty(), null)).Value.Count - 1;
                actualResponseCode = (await serviceImpl.Delete(deleteMessage, null)).Code;
                var allUsers = (await serviceImpl.GetAll(new Empty(), null)).Value;
                actualUser = allUsers.FirstOrDefault(user => user.Id == deleteMessage.Id);
                actualUserCount = allUsers.Count;
            });

            // Assert
            Assert.AreEqual(ResponseCode.Success, actualResponseCode);
            Assert.IsNull(actualUser);
            Assert.AreEqual(expectedUserCount, actualUserCount);
        }

        [Test]
        public async Task Test_Get_User()
        {
            // Arrange
            var user = (await GetServiceImpl().GetAll(new Empty(), null)).Value.First();

            // Action
            var actualUser = await GetServiceImpl().Get(new QueryUserMessage {Id = user.Id}, null);

            // Assert
            Assert.IsNotNull(actualUser);
            Assert.AreEqual(user.Id, actualUser.Id);
        }

        [Test]
        public async Task Test_Get_AllUsers()
        {
            // Action
            var actualResult = await GetServiceImpl().GetAll(new Empty(), null);

            // Assert
            Assert.IsNotEmpty(actualResult.Value);
        }

        #region private

        private UserServiceImpl GetServiceImpl()
        {
            return new UserServiceImpl(
                new Core.Services.UserService(
                    _config,
                    new UserRepository(_logger, _config)
                )
            );
        }

        private UserServiceImpl GetServiceImpl(MySqlConnection connection, MySqlTransaction transaction)
        {
            return new UserServiceImpl(
                new Core.Services.UserService(
                    _config,
                    new UserRepository(_logger, _config, connection, transaction)
                )
            );
        }

        private async Task RollBackTransactionScope(Func<MySqlConnection, MySqlTransaction, Task> action)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    await action(connection, transaction);
                    if (transaction.Connection != null)
                    {
                        await transaction.RollbackAsync();
                    }
                }
            }
        }

        #endregion
    }
}