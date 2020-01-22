#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

#endregion

namespace MockSite.Common.Data.Utilities
{
    public class SqlConnectionHelper
    {
        private readonly ILoggerProvider _loggerProvider;
        public SqlConnectionHelper(ILoggerProvider loggerProvider)
        {
            _loggerProvider = loggerProvider;
            var executedType = new HashSet<Type>();
            var iTaggableTypeInfo = typeof(ITaggable).GetTypeInfo();
            var allAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                a.FullName.StartsWith("MockSite", StringComparison.InvariantCultureIgnoreCase));

            Assembly.GetEntryAssembly().GetReferencedAssemblies();

            foreach (var current in allAssembly)
            {
                var types = current.DefinedTypes.Where(t => iTaggableTypeInfo.IsAssignableFrom(t));

                foreach (var type in types)
                {
                    foreach (var property in type.GetProperties(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (executedType.Contains(property.PropertyType) ||
                            property.GetCustomAttribute<TaggableAttribute>() == null) continue;
                        SqlMapper.AddTypeHandler(property.PropertyType, new TypeHandler());
                        executedType.Add(property.PropertyType);
                    }
                }
            }

            SqlMapper.AddTypeHandler(typeof(IEnumerable<int>), new TypeHandler());
        }

        public async ValueTask<int> ExecuteNonQueryAsync(
            MySqlConnection conn,
            string spName,
            DynamicParameters parameters,
            MySqlTransaction transaction
        )
        {
            var cmd = new CommandDefinition(spName, parameters, transaction, commandType: CommandType.StoredProcedure);
            var result = await new ProfiledDbConnection(_loggerProvider,conn).ExecuteAsync(cmd);

            return result;
        }

        public async Task<IEnumerable<T>> ExecuteQueryBySqlAsync<T>(
            MySqlConnection conn,
            string sql,
            DynamicParameters parameters,
            IDbTransaction transaction)
        {
            var cmd = new CommandDefinition(sql,parameters,transaction,commandType: CommandType.Text);
            var result = await new ProfiledDbConnection(_loggerProvider,conn).QueryAsync<T>(cmd);

            return result;
        }

        public async Task<T[]> ExecuteQueryAsync<T>(
            MySqlConnection conn,
            string spName,
            DynamicParameters parameters,
            IDbTransaction transaction
        )
        {
            var cmd = new CommandDefinition(spName, parameters, transaction, commandType: CommandType.StoredProcedure);
            var result = await new ProfiledDbConnection(_loggerProvider,conn).QueryAsync<T>(cmd);

            return result.ToArray();
        }

        public async Task<T> ExecuteQuerySingleAsync<T>(
            MySqlConnection conn,
            string spName,
            DynamicParameters parameters,
            IDbTransaction transaction
        )
        {
            var cmd = new CommandDefinition(spName, parameters, transaction, commandType: CommandType.StoredProcedure);
            var result = await new ProfiledDbConnection(_loggerProvider,conn).QuerySingleOrDefaultAsync<T>(cmd);

            return result;
        }

        public async Task<DbDataReader> ExecuteReaderAsync(
            MySqlConnection conn,
            string spName,
            IEnumerable<MySqlParameter> parameters,
            MySqlTransaction transaction
        )
        {
            using (var cmd = new ProfiledDbConnection(_loggerProvider,conn).CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = transaction;

                if (parameters != null) cmd.Parameters.AddRange(parameters.ToArray());

                return await cmd.ExecuteReaderAsync();
            }
        }
    }
}