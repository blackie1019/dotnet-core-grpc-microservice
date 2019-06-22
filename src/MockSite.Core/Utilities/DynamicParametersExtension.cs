using System.Data;
using Dapper;

namespace MockSite.Core.Utilities
{
    public static class DynamicParametersExtension
    {
        public static DynamicParameters AddInputParameter(
            this DynamicParameters parameters,
            string name,
            object value = null,
            DbType? dbType = null
        )
        {
            return AddParameter(parameters, name, value, dbType, ParameterDirection.Input);
        }

        public static DynamicParameters AddOutputParameter(
            this DynamicParameters parameters,
            string name,
            DbType? dbType = null
        )
        {
            return AddParameter(parameters, name, null, dbType, ParameterDirection.Output);
        }

        private static DynamicParameters AddParameter(
            DynamicParameters parameters,
            string name,
            object value = null,
            DbType? dbType = null,
            ParameterDirection? direction = null
        )
        {
            parameters.Add(name, value, dbType, direction);
            return parameters;
        }
    }
}