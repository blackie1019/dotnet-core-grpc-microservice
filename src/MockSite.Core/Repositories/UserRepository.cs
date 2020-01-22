#region

using System;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Data.Utilities;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using MockSite.Core.Utilities;
using MySql.Data.MySqlClient;

#endregion

namespace MockSite.Core.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly SqlConnectionHelper _helper;
        private readonly ILogger _logger;
        private readonly IRedisUserRepository _redisUserRepository;
        private readonly IMapper _mapper;

        public UserRepository(
            ILogger<UserRepository> logger,
            IConfiguration config,
            SqlConnectionHelper helper,
            IRedisUserRepository redisUserRepository
        )
        {
            _logger = logger;
            _connectionString = config.GetSection(DbConnectionConst.TestKey).Value;
            _helper = helper;
            _redisUserRepository = redisUserRepository;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserEntity, UserEntity>())
                .CreateMapper();

            logger.LogInformation("log init");
        }

        public Task<int> Create(UserEntity userEntity)
        {
            return ExecuteAsync(async (conn, transaction) =>
                {
                    var outReturnValueName = "OUT_ReturnValue";
                    var parameters = new DynamicParameters()
                        .AddInputParameter("IN_Code", userEntity.Code, DbType.String)
                        .AddInputParameter("IN_Name", userEntity.Name, DbType.String)
                        .AddInputParameter("IN_Email", userEntity.Email, DbType.String)
                        .AddInputParameter("IN_Password", userEntity.Password, DbType.String)
                        .AddOutputParameter(outReturnValueName, DbType.Int32);
                    var createdUserId = await _helper.ExecuteQuerySingleAsync<int>(
                        conn,
                        StoreProcedureName.CreateUser,
                        parameters,
                        transaction
                    );
                    var returnCode = parameters.Get<int>(outReturnValueName);

                    if (returnCode != 1)
                        throw new Exception(returnCode.ToString());

                    var createdUser = _mapper.Map<UserEntity>(userEntity);
                    createdUser.Id = createdUserId;
                    await _redisUserRepository.Create(createdUser);

                    return createdUserId;
                },
                createTransaction: true);
        }

        public Task Update(UserEntity userEntity)
        {
            return ExecuteAsync(async (conn, transaction) =>
                {
                    var parameters = new DynamicParameters()
                        .AddInputParameter("IN_Id", userEntity.Id, DbType.Int32)
                        .AddInputParameter("IN_Name", userEntity.Name, DbType.String)
                        .AddInputParameter("IN_Email", userEntity.Email, DbType.String)
                        .AddOutputParameter("OUT_ReturnValue");

                    await _helper.ExecuteNonQueryAsync
                    (
                        conn,
                        StoreProcedureName.UpdateUser,
                        parameters,
                        transaction
                    );

                    return _redisUserRepository.Update(userEntity);
                },
                createTransaction: true);
        }

        public Task Delete(int id)
        {
            return ExecuteAsync(async (conn, transaction) =>
            {
                var parameters = new DynamicParameters()
                    .AddInputParameter("IN_Id", id, DbType.Int32)
                    .AddOutputParameter("OUT_ReturnValue");

                await _helper.ExecuteNonQueryAsync
                (
                    conn,
                    StoreProcedureName.DeleteUser,
                    parameters,
                    transaction
                );

                return _redisUserRepository.Delete(id);
            }, createTransaction: true);
        }

        public Task<UserEntity[]> GetAll()
        {
            return ExecuteAsync(async (conn, transaction) =>
            {
                var parameters = new DynamicParameters()
                    .AddOutputParameter("OUT_ReturnValue");

                var entities = await _helper.ExecuteQueryAsync<UserEntity>
                (
                    conn,
                    StoreProcedureName.GetUsers,
                    parameters,
                    transaction
                );

                if (_redisUserRepository == null) return entities;

                await _redisUserRepository.DeleteAll();

                if (entities.Length == 0)
                    return entities;
                foreach (var entity in entities)
                    await _redisUserRepository.Create(entity);

                return entities;
            });
        }

        public async Task<UserEntity> GetById(int id)
        {
            var user = await _redisUserRepository.GetById(id);

            if (user != null) return user;

            return await ExecuteAsync((conn, transaction) =>
            {
                var parameters = new DynamicParameters()
                    .AddInputParameter("IN_Id", id, DbType.Int32)
                    .AddOutputParameter("OUT_ReturnValue");

                return _helper.ExecuteQuerySingleAsync<UserEntity>
                (
                    conn,
                    StoreProcedureName.GetUser,
                    parameters,
                    transaction
                );
            });
        }

        public Task<UserEntity[]> GetByCondition(string code = null, string name = null,
            string email = null)
        {
            return ExecuteAsync(async (conn, transaction) =>
            {
                var parameters = new DynamicParameters()
                    .AddInputParameter("IN_Code", code, DbType.String)
                    .AddInputParameter("IN_Name", name, DbType.String)
                    .AddInputParameter("IN_Email", email, DbType.String)
                    .AddOutputParameter("OUT_ReturnValue");

                var entities = await _helper.ExecuteQueryAsync<UserEntity>
                (
                    conn,
                    StoreProcedureName.GetUsersByCondition,
                    parameters,
                    transaction
                );

                if (entities.Length == 0) return entities;

                await _redisUserRepository.DeleteAll();
                foreach (var entity in entities)
                    await _redisUserRepository.Create(entity);

                return entities;
            });
        }

        private async Task<T> ExecuteAsync<T>(Func<MySqlConnection, MySqlTransaction, Task<T>> mainLogic,
            Action rollbackLogic = null,
            bool createTransaction = false)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                MySqlTransaction tran = null;
                if (createTransaction)
                    tran = await connection.BeginTransactionAsync();

                try
                {
                    var result = await mainLogic(connection, tran);
                    if (tran != null)
                        await tran.CommitAsync();

                    return result;
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        await tran.RollbackAsync();
                    rollbackLogic?.Invoke();
                    _logger.LogError(ex, ex.Message);

                    throw;
                }
                finally
                {
                    tran?.Dispose();
                }
            }
        }

        #region StoreProcedureName

        private struct StoreProcedureName
        {
            public const string CreateUser = "sp_User_CreateUser";
            public const string GetUsers = "sp_User_GetUsers";
            public const string GetUser = "sp_User_GetUser";
            public const string UpdateUser = "sp_User_UpdateUser";
            public const string DeleteUser = "sp_User_DeleteUser";
            public const string GetUsersByCondition = "sp_User_GetUsersByCondition";
        }

        #endregion
    }
}