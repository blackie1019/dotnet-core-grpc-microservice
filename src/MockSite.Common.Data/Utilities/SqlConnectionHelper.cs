#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

#endregion

namespace MockSite.Common.Data.Utilities
{
    public static class SqlConnectionHelper
    {
        static SqlConnectionHelper()
        {
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

        public static async ValueTask<int> ExecuteNonQueryAsync(
            this MySqlConnection conn,
            string spName,
            DynamicParameters parameters,
            MySqlTransaction transaction
        )
        {
            var cmd = new CommandDefinition(spName, parameters, transaction, commandType: CommandType.StoredProcedure);
            var result = await new ProfiledDbConnection(conn).ExecuteAsync(cmd);
            return result;
        }

        public static async Task<IEnumerable<T>> ExecuteQueryAsync<T>(
            this MySqlConnection conn,
            string spName,
            DynamicParameters parameters,
            IDbTransaction transaction
        )
        {
            var cmd = new CommandDefinition(spName, parameters, transaction, commandType: CommandType.StoredProcedure);
            var result = await new ProfiledDbConnection(conn).QueryAsync<T>(cmd);
            return result;
        }

        public static async Task<T> ExecuteQuerySingleAsync<T>(
            this MySqlConnection conn,
            string spName,
            DynamicParameters parameters,
            IDbTransaction transaction
        )
        {
            var cmd = new CommandDefinition(spName, parameters, transaction, commandType: CommandType.StoredProcedure);
            var result = await new ProfiledDbConnection(conn).QuerySingleOrDefaultAsync<T>(cmd);
            return result;
        }

        public static async Task<DbDataReader> ExecuteReaderAsync(
            this MySqlConnection conn,
            string spName,
            IEnumerable<MySqlParameter> parameters,
            MySqlTransaction transaction
        )
        {
            using (var cmd = new ProfiledDbConnection(conn).CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = transaction;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                return await cmd.ExecuteReaderAsync();
            }
        }
    }
}