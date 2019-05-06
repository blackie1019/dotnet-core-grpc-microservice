using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MockSite.Common.Logging.Utilities;

namespace MockSite.Common.Data
{
    public class ProfiledDbCommand : DbCommand
    {
        private DbCommand _command;
        private DbConnection _connection;
        private DbTransaction _transaction;

        public ProfiledDbCommand(DbCommand command, DbConnection connection)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));

            if (connection != null)
            {
                _connection = connection;
                UnwrapAndAssignConnection(connection);
            }
        }

        public override string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }

        protected override DbConnection DbConnection
        {
            get => _connection;
            set
            {
                _connection = value;
                UnwrapAndAssignConnection(value);
            }
        }

        private void UnwrapAndAssignConnection(DbConnection value)
        {
            if (value is ProfiledDbConnection profiledConn)
            {
                _command.Connection = profiledConn.WrappedConnection;
            }
            else
            {
                _command.Connection = value;
            }
        }

        protected override DbParameterCollection DbParameterCollection => _command.Parameters;

        protected override DbTransaction DbTransaction
        {
            get => _transaction;
            set
            {
                _transaction = value;
                var awesomeTran = value as ProfiledDbTransaction;
                _command.Transaction = awesomeTran == null ? value : awesomeTran.WrappedTransaction;
            }
        }

        public override bool DesignTimeVisible
        {
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }

        private void LogElasped(Stopwatch watch)
        {
            watch.Stop();
            var perfDetail = LoggerHelper.Instance.GetPerformanceDetail();
            
            perfDetail.Parameter = GetParametersForLogging();
            perfDetail.Target = _command.CommandText;
            perfDetail.Duration = watch.ElapsedMilliseconds;
            LoggerHelper.Instance.Performance(perfDetail);
        }

        private Dictionary<string, string> GetParametersForLogging()
        {
            if (_command.Parameters.Count > 0)
            {
                var parameterDic = new Dictionary<string, string>();
                foreach (DbParameter parameter in _command.Parameters)
                {
                    parameterDic.Add(parameter.ParameterName, parameter.Value?.ToString());
                }

                return parameterDic;
            }

            return null;
        }

        private void LogElasped(Stopwatch watch, string commandText, Dictionary<string, string> parameterDic)
        {
            watch.Stop();
            
            var perfDetail = LoggerHelper.Instance.GetPerformanceDetail();
            perfDetail.Parameter = parameterDic;
            perfDetail.Target = commandText;
            perfDetail.Duration = watch.ElapsedMilliseconds;
            LoggerHelper.Instance.Performance(perfDetail);
        }

        protected virtual DbDataReader CreateDbDataReader(DbDataReader original, Action callback) => new ProfiledDbDataReader(original, callback);

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            DbDataReader result;
            try
            {
                var parametersForLogging = GetParametersForLogging();
                var commandText = _command.CommandText;
                var watch = new Stopwatch();
                watch.Start();
                result = _command.ExecuteReader(behavior);
                result = CreateDbDataReader(result, () => LogElasped(watch, commandText, parametersForLogging));
            }
            catch (Exception ex)
            {
                var exDetail = LoggerHelper.Instance.GetExceptionDetail(ex);
                LoggerHelper.Instance.Error(exDetail);
                throw;
            }

            return result;
        }
        
        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            DbDataReader result;
            var parametersForLogging = GetParametersForLogging();
            var commandText = _command.CommandText;
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                result = await _command.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);
                result = CreateDbDataReader(result, () => LogElasped(watch, commandText, parametersForLogging));
            }
            catch (Exception ex)
            {
                var exDetail = LoggerHelper.Instance.GetExceptionDetail(ex);
                LoggerHelper.Instance.Error(exDetail);
                throw;
            }

            return result;
        }

        public override int ExecuteNonQuery()
        {
            int result;
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                result = _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var exDetail = LoggerHelper.Instance.GetExceptionDetail(ex);
                LoggerHelper.Instance.Error(exDetail);
                throw;
            }
            finally
            {
                LogElasped(watch);
            }

            return result;
        }

        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            int result;
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                result = await _command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var exDetail = LoggerHelper.Instance.GetExceptionDetail(ex);
                LoggerHelper.Instance.Error(exDetail);
                throw;
            }
            finally
            {
                LogElasped(watch);
            }

            return result;
        }

        public override object ExecuteScalar()
        {
            object result;
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                result = _command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                var exDetail = LoggerHelper.Instance.GetExceptionDetail(ex);
                LoggerHelper.Instance.Error(exDetail);
                throw;
            }
            finally
            {
                LogElasped(watch);
            }

            return result;
        }

        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            object result;
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                result = await _command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var exDetail = LoggerHelper.Instance.GetExceptionDetail(ex);
                LoggerHelper.Instance.Error(exDetail);
                throw;
            }
            finally
            {
                LogElasped(watch);
            }

            return result;
        }

        public override void Cancel() => _command.Cancel();

        public override void Prepare() => _command.Prepare();

        protected override DbParameter CreateDbParameter() => _command.CreateParameter();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _command?.Dispose();
            }

            _command = null;
            base.Dispose(disposing);
        }

        public DbCommand InternalCommand => _command;
    }
}