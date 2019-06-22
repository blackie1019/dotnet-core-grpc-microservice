#region

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Data.Utilities;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using MockSite.Core.Utilities;
using MySql.Data.MySqlClient;

#endregion

namespace MockSite.Core.Repositories
{
    public class UserRepository : IRepository
    {
        private readonly string _connectionString;
        private readonly MySqlConnection _connection;
        private readonly MySqlTransaction _transaction;

        public UserRepository(
            ILogger<UserRepository> logger,
            IConfiguration config,
            MySqlConnection conn = null,
            MySqlTransaction transaction = null
        )
        {
            _connectionString = config.GetSection(DbConnectionConst.TestKey).Value;
            _connection = conn;
            _transaction = transaction;
            logger.LogInformation("log init");
        }

        public async Task Create(UserDto userDto)
        {
            async Task Execute(MySqlConnection conn)
            {
                var parameters = new DynamicParameters()
                    .AddInputParameter("IN_Code", userDto.Code, DbType.String)
                    .AddInputParameter("IN_Name", userDto.Name, DbType.String)
                    .AddInputParameter("IN_Email", userDto.Email, DbType.String)
                    .AddInputParameter("IN_Password", userDto.Password, DbType.String)
                    .AddOutputParameter("OUT_ReturnValue");
                await conn.ExecuteNonQueryAsync(
                    StoreProcedureName.CreateUser,
                    parameters,
                    _transaction
                );
            }

            if (_connection == null)
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    await Execute(conn);
                }
            }
            else
            {
                await Execute(_connection);
            }
        }

        public async Task Update(UserDto userDto)
        {
            async Task Execute(MySqlConnection conn)
            {
                var parameters = new DynamicParameters()
                    .AddInputParameter("IN_Id", userDto.Id, DbType.Int32)
                    .AddInputParameter("IN_Name", userDto.Name, DbType.String)
                    .AddInputParameter("IN_Email", userDto.Email, DbType.String)
                    .AddOutputParameter("OUT_ReturnValue");
                await conn.ExecuteNonQueryAsync
                (
                    StoreProcedureName.UpdateUser,
                    parameters,
                    _transaction
                );
            }

            if (_connection == null)
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    await Execute(conn);
                }
            }
            else
            {
                await Execute(_connection);
            }
        }

        public async Task Delete(int id)
        {
            async Task Execute(MySqlConnection conn)
            {
                var parameters = new DynamicParameters()
                    .AddInputParameter("IN_Id", id, DbType.Int32)
                    .AddOutputParameter("OUT_ReturnValue");
                await conn.ExecuteNonQueryAsync
                (
                    StoreProcedureName.DeleteUser,
                    parameters,
                    _transaction
                );
            }

            if (_connection == null)
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    await Execute(conn);
                }
            }
            else
            {
                await Execute(_connection);
            }
        }

        public async Task<IEnumerable<UserEntity>> GetAll()
        {
            async Task<IEnumerable<UserEntity>> Execute(MySqlConnection conn)
            {
                var parameters = new DynamicParameters()
                    .AddOutputParameter("OUT_ReturnValue");
                var result = await conn.ExecuteQueryAsync<UserEntity>
                (
                    StoreProcedureName.GetUsers,
                    parameters,
                    _transaction
                );
                return result;
            }

            if (_connection != null) return await Execute(_connection);
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                return await Execute(conn);
            }
        }

        public async Task<UserEntity> GetById(int id)
        {
            async Task<UserEntity> Execute(MySqlConnection conn)
            {
                var parameters = new DynamicParameters()
                    .AddInputParameter("IN_Id", id, DbType.Int32)
                    .AddOutputParameter("OUT_ReturnValue");
                var result = await conn.ExecuteQuerySingleAsync<UserEntity>
                (
                    StoreProcedureName.GetUser,
                    parameters,
                    _transaction
                );
                return result;
            }

            if (_connection != null) return await Execute(_connection);
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                return await Execute(conn);
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
        }

        #endregion
    }
}