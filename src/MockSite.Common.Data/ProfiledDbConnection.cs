#region

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;

using IsolationLevel = System.Data.IsolationLevel;

#endregion

namespace MockSite.Common.Data
{
    public sealed class ProfiledDbConnection : DbConnection
    {
        private DbConnection _connection;
        private readonly ILoggerProvider _loggerProvider;

        public ProfiledDbConnection(ILoggerProvider loggerProvider, DbConnection connection)
        {
            _loggerProvider = loggerProvider;
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _connection.StateChange += StateChangeHandler;
        }

        public DbConnection WrappedConnection
        {
            get { return _connection; }
        }

        public override string ConnectionString
        {
            get { return _connection.ConnectionString; }
            set { _connection.ConnectionString = value; }
        }

        public override int ConnectionTimeout
        {
            get { return _connection.ConnectionTimeout; }
        }

        public override string Database
        {
            get { return _connection.Database; }
        }

        public override string DataSource
        {
            get { return _connection.DataSource; }
        }

        public override string ServerVersion
        {
            get { return _connection.ServerVersion; }
        }

        public override ConnectionState State
        {
            get { return _connection.State; }
        }

        protected override bool CanRaiseEvents
        {
            get { return true; }
        }

        public override void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _connection.Close();
        }

        public override void Open()
        {
            _connection.Open();
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return _connection.OpenAsync(cancellationToken);
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new ProfiledDbTransaction(_connection.BeginTransaction(isolationLevel), this);
        }

        protected override DbCommand CreateDbCommand()
        {
            return new ProfiledDbCommand(_loggerProvider.CreateLogger(nameof(ProfiledDbCommand)),
                _connection.CreateCommand(), this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _connection != null)
            {
                _connection.StateChange -= StateChangeHandler;
                _connection.Dispose();
            }

            base.Dispose(disposing);
            _connection = null;
        }

        private void StateChangeHandler(object sender, StateChangeEventArgs stateChangeEventArguments)
        {
            OnStateChange(stateChangeEventArguments);
        }

        public override void EnlistTransaction(Transaction transaction)
        {
            _connection.EnlistTransaction(transaction);
        }

        public override DataTable GetSchema()
        {
            return _connection.GetSchema();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return _connection.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return _connection.GetSchema(collectionName, restrictionValues);
        }
    }
}