#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MockSite.Common.Logging;
using MockSite.Common.Logging.Utilities.LogDetail;

#endregion

namespace MockSite.Common.Data
{
    public sealed class ProfiledDbCommand : DbCommand
    {
        private DbCommand _command;
        private DbConnection _connection;
        private DbTransaction _transaction;
        private readonly ILogger _logger;

        public ProfiledDbCommand(ILogger logger, DbCommand command, DbConnection connection)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));

            _logger = logger;
            
            if (connection == null) return;
            _connection = connection;
            UnwrapAndAssignConnection(connection);
        }

        public override string CommandText
        {
            get { return _command.CommandText; }
            set { _command.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get { return _command.CommandTimeout; }
            set { _command.CommandTimeout = value; }
        }

        public override CommandType CommandType
        {
            get { return _command.CommandType; }
            set { _command.CommandType = value; }
        }

        protected override DbConnection DbConnection
        {
            get { return _connection; }
            set
            {
                _connection = value;
                UnwrapAndAssignConnection(value);
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _command.Parameters; }
        }

        protected override DbTransaction DbTransaction
        {
            get { return _transaction; }
            set
            {
                _transaction = value;
                _command.Transaction = !(value is ProfiledDbTransaction awesomeTran)
                    ? value
                    : awesomeTran.WrappedTransaction;
            }
        }

        public override bool DesignTimeVisible
        {
            get { return _command.DesignTimeVisible; }
            set { _command.DesignTimeVisible = value; }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get { return _command.UpdatedRowSource; }
            set { _command.UpdatedRowSource = value; }
        }

        public DbCommand InternalCommand
        {
            get { return _command; }
        }
        
        private void LogElapsed(Stopwatch watch)
        {
            watch.Stop();
            var detail = new PerformanceDetail
            {
                Parameter = GetParametersForLogging(),
                Target = _command.CommandText,
                Duration = watch.ElapsedMilliseconds
            };

            _logger.Performance(detail);
        }
        
        private void LogElapsed(Stopwatch watch, string commandText, Dictionary<string, string> parameterDic)
        {
            watch.Stop();

            var perfDetail = new PerformanceDetail
            {
                Parameter = parameterDic, Target = commandText, Duration = watch.ElapsedMilliseconds
            };
            _logger.Performance(perfDetail);
        }

        private void UnwrapAndAssignConnection(DbConnection value)
        {
            if (value is ProfiledDbConnection profiledConn)
                _command.Connection = profiledConn.WrappedConnection;
            else
                _command.Connection = value;
        }

        private Dictionary<string, string> GetParametersForLogging()
        {
            return _command.Parameters.Count <= 0 ? null : _command.Parameters.Cast<DbParameter>().ToDictionary(parameter => parameter.ParameterName, parameter => parameter.Value?.ToString());
        }


        private DbDataReader CreateDbDataReader(DbDataReader original, Action callback)
        {
            return new ProfiledDbDataReader(original, callback);
        }

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
                result = CreateDbDataReader(result, () => LogElapsed(watch, commandText, parametersForLogging));
            }
            catch (Exception ex)
            {
                DealWithException(ex);

                throw;
            }

            return result;
        }

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior,
            CancellationToken cancellationToken)
        {
            DbDataReader result;
            try
            {
                var parametersForLogging = GetParametersForLogging();
                var commandText = _command.CommandText;
                var watch = new Stopwatch();
                watch.Start();
                result = await _command.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);
                result = CreateDbDataReader(result, () => LogElapsed(watch, commandText, parametersForLogging));
            }
            catch (Exception ex)
            {
                DealWithException(ex);

                throw;
            }

            return result;
        }

        private void DealWithException(Exception ex)
        {
            var exDetail = new ErrorDetail()
            {
                StackTrace = ex.ToString()
            };
            _logger.Error(exDetail);
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
                DealWithException(ex);

                throw;
            }
            finally
            {
                LogElapsed(watch);
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
                DealWithException(ex);

                throw;
            }
            finally
            {
                LogElapsed(watch);
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
                DealWithException(ex);

                throw;
            }
            finally
            {
                LogElapsed(watch);
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
                DealWithException(ex);

                throw;
            }
            finally
            {
                LogElapsed(watch);
            }

            return result;
        }

        public override void Cancel()
        {
            _command.Cancel();
        }

        public override void Prepare()
        {
            _command.Prepare();
        }

        protected override DbParameter CreateDbParameter()
        {
            return _command.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _command?.Dispose();

            _command = null;
            base.Dispose(disposing);
        }
    }
}