using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Infrastructure.Helpers;
using MockSite.Core.Constants;
using MockSite.Core.Entities;
using MySql.Data.MySqlClient;

namespace MockSite.Core.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly MySqlConnection _testConnection;
        private readonly MySqlTransaction _transaction;

        private static readonly string ConnString =
            AppSettingsHelper.Instance.GetValueFromKey(DbConnectionConst.TestDbKey);

        public UserRepository(MySqlConnection conn = null, MySqlTransaction transaction = null)
        {
            _testConnection = conn;
            _transaction = transaction;
        }

        public async Task Create(User instance)
        {
            // Calling SP with MySqlConnector(https://github.com/mysql-net/MySqlConnector)
            async Task Execute(MySqlConnection conn)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_User_CreateUser";
                    cmd.Parameters.AddWithValue("IN_Code", instance.Code);
                    cmd.Parameters.AddWithValue("IN_DisplayKey", instance.DisplayKey);
                    cmd.Parameters.AddWithValue("IN_OrderNo", instance.OrderNo);
                    cmd.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "OUT_ReturnValue", MySqlDbType = MySqlDbType.Int32,
                        Direction = ParameterDirection.Output
                    });

                    if (_transaction != null)
                    {
                        cmd.Transaction = _transaction;
                    }

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            if (_testConnection == null)
            {
                using (var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    await Execute(conn);
                }
            }
            else
            {
                await Execute(_testConnection);
            }
        }

        public async Task Update(User instance)
        {
            // Calling SP with MySqlConnector(https://github.com/mysql-net/MySqlConnector)
            async Task Execute(MySqlConnection conn)
            {
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sp_User_UpdateUser";
                        cmd.Parameters.AddWithValue("IN_Code", instance.Code);
                        cmd.Parameters.AddWithValue("IN_DisplayKey", instance.DisplayKey);
                        cmd.Parameters.AddWithValue("IN_OrderNo", instance.OrderNo);
                        cmd.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "OUT_ReturnValue", MySqlDbType = MySqlDbType.Int32,
                            Direction = ParameterDirection.Output
                        });

                        if (_transaction != null)
                        {
                            cmd.Transaction = _transaction;
                        }

                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            if (_testConnection == null)
            {
                using (var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    await Execute(conn);
                }
            }
            else
            {
                await Execute(_testConnection);
            }
        }

        public async Task Delete(User instance)
        {
            // Calling SP with MySqlConnector(https://github.com/mysql-net/MySqlConnector)
            async Task Execute(MySqlConnection conn)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_User_DeleteUser";
                    cmd.Parameters.AddWithValue("IN_Code", instance.Code);
                    if (_transaction != null)
                    {
                        cmd.Transaction = _transaction;
                    }
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            if (_testConnection == null)
            {
                using (var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    await Execute(conn);
                }
            }
            else
            {
                await Execute(_testConnection);
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            // Calling SP with Dapper & MySqlConnector(https://github.com/mysql-net/MySqlConnector)
            async Task<IEnumerable<User>> Execute(MySqlConnection conn)
            {
                var parameters = new DynamicParameters();
                parameters.Add("OUT_ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.Output);

                return await conn.QueryAsync<User>("sp_User_GetUsers", parameters, commandType: CommandType.StoredProcedure,
                    transaction: _transaction);
            }

            if (_testConnection == null)
            {
                using (var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    return await Execute(conn);
                }
            }
            else
            {
                return await Execute(_testConnection);
            }
        }

        public async Task<User> GetByPk(object userCode)
        {
            // Use Dapper & Dapper.Contrib
            async Task<User> Execute(MySqlConnection conn)
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("IN_Code", userCode);
                    parameters.Add("OUT_ReturnValue", dbType: DbType.Object, direction: ParameterDirection.Output);

                    return await conn.QuerySingleAsync<User>("sp_User_GetUser", parameters,
                        commandType: CommandType.StoredProcedure,transaction:_transaction);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            
            if (_testConnection == null)
            {
                using (var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    return await Execute(conn);
                }
            }
            else
            {
                return await Execute(_testConnection);
            }
        }
    }
}