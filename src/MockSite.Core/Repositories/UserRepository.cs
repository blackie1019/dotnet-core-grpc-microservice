using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using MockSite.Common.Core.Utilities;
using MockSite.Common.Data.Utilities;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MySql.Data.MySqlClient;
using DbConnectionConst = MockSite.Common.Core.Constants.DomainService.DbConnectionConst;


namespace MockSite.Core.Repositories
{
    public class UserRepository : IRepository
    {
        private readonly MySqlConnection _testConnection;
        private readonly MySqlTransaction _transaction;

        private static readonly string ConnString =
            AppSettingsHelper.Instance.GetValueFromKey(DbConnectionConst.TestKey);
            

        public UserRepository(MySqlConnection conn = null, MySqlTransaction transaction = null)
        {
            _testConnection = conn;
            _transaction = transaction;
        }

        public async Task Create(UserDTO userDTO)
        {
            async Task Execute(MySqlConnection conn)
            {
                var parameters = GenerateParameters();
                await conn.ExecuteNonQueryAsync
                (
                    StoreProcedureName.sp_User_CreateUser,
                    parameters,
                    _transaction);
            }

            if (_testConnection == null)
            {
                using (var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    await Execute(conn);
                }
            }

            await Execute(_testConnection);

            #region - Parameters -

            List<MySqlParameter> GenerateParameters()
            {
                var parameters = new List<MySqlParameter>
                {
                    new MySqlParameter
                    {
                        ParameterName = "IN_Code",
                        Value = userDTO.Code,
                        MySqlDbType = MySqlDbType.Int32,
                        Direction = ParameterDirection.Input
                    },
                    new MySqlParameter
                    {
                        ParameterName = "IN_DisplayKey",
                        Value = userDTO.DisplayKey,
                        MySqlDbType = MySqlDbType.VarChar,
                        Direction = ParameterDirection.Input
                    },
                    new MySqlParameter
                    {
                        ParameterName = "IN_OrderNo",
                        Value = userDTO.OrderNo,
                        MySqlDbType = MySqlDbType.Int32,
                        Direction = ParameterDirection.Input
                    },

                    new MySqlParameter
                    {
                        ParameterName = "OUT_ReturnValue",
                        MySqlDbType = MySqlDbType.Int32,
                        Direction = ParameterDirection.Output
                    }
                };
                return parameters;
            }

            #endregion
        }

        public async Task Update(UserDTO userDTO)
        {
            async Task Execute(MySqlConnection conn)
            {
                var parameters = GenerateParameters();

                await conn.ExecuteNonQueryAsync
                (
                    StoreProcedureName.sp_User_UpdateUser,
                    parameters,
                    _transaction
                );
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

            #region Parameters

            List<MySqlParameter> GenerateParameters()
            {
                var parameters = new List<MySqlParameter>
                {
                    new MySqlParameter
                    {
                        ParameterName = "IN_Code",
                        Value = userDTO.Code,
                        MySqlDbType = MySqlDbType.String,
                        Direction = ParameterDirection.Input
                    },
                    new MySqlParameter
                    {
                        ParameterName = "IN_DisplayKey",
                        Value = userDTO.DisplayKey,
                        MySqlDbType = MySqlDbType.Int16,
                        Direction = ParameterDirection.Input
                    },
                    new MySqlParameter
                    {
                        ParameterName = "IN_OrderNo",
                        Value = userDTO.OrderNo,
                        MySqlDbType = MySqlDbType.Int16,
                        Direction = ParameterDirection.Input
                    },
                    new MySqlParameter
                    {
                        ParameterName = "OUT_ReturnValue",
                        MySqlDbType = MySqlDbType.Int32,
                        Direction = ParameterDirection.Output
                    }
                };
                return parameters;
            }

            #endregion
        }

        public async Task Delete(UserDTO userDTO)
        {
            async Task Execute(MySqlConnection conn)
            {
                var parameters = GenerateParameters();

                await conn.ExecuteNonQueryAsync
                (
                    StoreProcedureName.sp_User_DeleteUser,
                    parameters,
                    _transaction
                );
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

            #region Parameters

            List<MySqlParameter> GenerateParameters()
            {
                var parameters = new List<MySqlParameter>
                {
                    new MySqlParameter
                    {
                        ParameterName = "IN_Code",
                        Value = userDTO.Code,
                        MySqlDbType = MySqlDbType.Int32,
                        Direction = ParameterDirection.Input
                    },
                    new MySqlParameter
                    {
                        ParameterName = "OUT_ReturnValue",
                        MySqlDbType = MySqlDbType.Int32,
                        Direction = ParameterDirection.Output
                    }
                };
                return parameters;
            }

            #endregion
        }

        public async Task<IEnumerable<UserEntity>> GetAll()
        {
            async Task<IEnumerable<UserEntity>> Execute(MySqlConnection conn)
            {
                var parameters = GenerateParameters();

                var result = await conn.ExecuteQueryAsync<UserEntity>
                (
                    StoreProcedureName.sp_User_GetUsers,
                    parameters,
                    _transaction
                );

                return result;
            }

            if (_testConnection == null)
            {
                using (var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    return await Execute(conn);
                }
            }

            return await Execute(_testConnection);

            #region Parameters

            DynamicParameters GenerateParameters()
            {
                var parameters = new DynamicParameters();
                parameters.Add("OUT_ReturnValue", dbType: DbType.Object, direction: ParameterDirection.Output);
                return parameters;
            }

            #endregion
        }

        public async Task<UserEntity> GetByCode(object userCode)
        {
            async Task<UserEntity> Execute(MySqlConnection conn)
            {
                var parameters = GenerateParameters();

                var result = await conn.ExecuteQuerySingleAsync<UserEntity>
                (
                    StoreProcedureName.sp_User_GetUser,
                    parameters,
                    _transaction
                );

                return result;
            }

            if (_testConnection == null)
            {
                using (var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    return await Execute(conn);
                }
            }

            return await Execute(_testConnection);


            #region Parameters

            DynamicParameters GenerateParameters()
            {
                var parameters = new DynamicParameters();
                parameters.Add("IN_Code", userCode, DbType.Int32, ParameterDirection.Input);
                parameters.Add("OUT_ReturnValue", dbType: DbType.Object, direction: ParameterDirection.Output);
                return parameters;
            }

            #endregion
        }

        #region StoreProcedureName

        private struct StoreProcedureName
        {
            public static readonly string sp_User_CreateUser = "sp_User_CreateUser";
            public static readonly string sp_User_DeleteUser = "sp_User_DeleteUser";
            public static readonly string sp_User_GetUser = "sp_User_GetUser";
            public static readonly string sp_User_GetUsers = "sp_User_GetUsers";
            public static readonly string sp_User_UpdateUser = "sp_User_UpdateUser";
        }

        #endregion
    }
}