using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Core.Repositories;
using MockSite.DomainService;
using MockSite.Message;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using NSubstitute;

namespace MockSite.Test
{
    [Parallelizable]
    [TestFixture]
    public class SportTest
    {
        private ILogger<UserRepository> logger;
        private IConfiguration config;


        [OneTimeSetUp]
        public void SetUp()
        {
            logger = Substitute.For<ILogger<UserRepository>>();
            config = Substitute.For<IConfiguration>();
            config.GetSection(DbConnectionConst.TestKey).Value
                .Returns("Server=localhost; Port=3326; Database=TestDB;Uid=root;Pwd=pass.123;");
        }

        [Test]
        public async Task Test_Create_User()
        {
            // Arrange
            var displayKey = "TestNew998";
            var code = 998;
            var orderNo = 6;
            var expected = new User()
            {
                Code = code,
                DisplayKey = displayKey,
                OrderNo = orderNo
            };
            User actual;

            // Action
            using (var testConnection =
                new MySqlConnection(ConsulSettingHelper.Instance.GetValueFromKey(DbConnectionConst.TestKey)))
            {
                await testConnection.OpenAsync();
                using (var transaction = testConnection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                {
                    var serviceImpl = GetServiceImpl(testConnection, transaction);
                    await serviceImpl.Create(expected, null);
                    actual = await serviceImpl.Get(new UserCode {Code = code}, null);
                    transaction.Rollback();
                }
            }

            var currentSport = await GetServiceImpl().Get(new UserCode {Code = code}, null);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreNotEqual(expected, currentSport);
        }

        [Test]
        public async Task Test_Update_User_Test99_ChangeDisplayKey_AddDateTime()
        {
            // Arrange
            var displayKey = $"Test99{DateTime.Now:s}";
            var code = 99;
            var orderNo = 6;
            var expected = new User()
            {
                Code = code,
                DisplayKey = displayKey,
                OrderNo = orderNo
            };
            User actual;

            // Action
            using (var testConnection =
                new MySqlConnection(ConsulSettingHelper.Instance.GetValueFromKey(DbConnectionConst.TestKey)))
            {
                await testConnection.OpenAsync();
                using (var transaction = testConnection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                {
                    var serviceImpl = GetServiceImpl(testConnection, transaction);
                    await serviceImpl.Update(expected, null);
                    actual = await serviceImpl.Get(new UserCode {Code = expected.Code}, null);
                    transaction.Rollback();
                }
            }

            var currentSport = await GetServiceImpl().Get(new UserCode {Code = expected.Code}, null);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreNotEqual(currentSport, actual);
        }

        [Test]
        public async Task Test_Get_User_99()
        {
            // Arrange
            var code = 99;
            var expected = new User
            {
                Code = 99,
                DisplayKey = "BLACKIE",
                OrderNo = 99
            };

            // Action
            var actual = await GetServiceImpl().Get(new UserCode {Code = code}, null);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task Test_Get_AllUsers()
        {
            var actual = await GetServiceImpl().GetAll(new Empty(), null);

            Assert.IsTrue(actual.Value.Count > 0);
        }

        #region private

        private UserServiceImpl GetServiceImpl()
        {
            return new UserServiceImpl(new Core.Services.UserService(new UserRepository(logger, config)));
        }

        private UserServiceImpl GetServiceImpl(MySqlConnection connection, MySqlTransaction transaction)
        {
            return new UserServiceImpl(
                new Core.Services.UserService(new UserRepository(logger, config, connection, transaction)));
        }

        #endregion
    }
}