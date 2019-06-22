#region

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace MockSite.Common.Data
{
    public sealed class ProfiledDbConnection : DbConnection
    {
        private DbConnection _connection;

        public ProfiledDbConnection(DbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _connection.StateChange += StateChangeHandler;
        }

        public DbConnection WrappedConnection => _connection;

        public override string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        public override int ConnectionTimeout => _connection.ConnectionTimeout;

        public override string Database => _connection.Database;

        public override string DataSource => _connection.DataSource;

        public override string ServerVersion => _connection.ServerVersion;

        public override ConnectionState State => _connection.State;

        public override void ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);

        public override void Close() => _connection.Close();

        public override void Open() => _connection.Open();

        public override Task OpenAsync(CancellationToken cancellationToken) => _connection.OpenAsync(cancellationToken);

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new ProfiledDbTransaction(_connection.BeginTransaction(isolationLevel), this);
        }

        private DbCommand CreateDbCommand(DbCommand original) => new ProfiledDbCommand(original, this);

        protected override DbCommand CreateDbCommand() => CreateDbCommand(_connection.CreateCommand());

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

        protected override bool CanRaiseEvents => true;

        public override void EnlistTransaction(System.Transactions.Transaction transaction) =>
            _connection.EnlistTransaction(transaction);

        public override DataTable GetSchema() => _connection.GetSchema();

        public override DataTable GetSchema(string collectionName) => _connection.GetSchema(collectionName);

        public override DataTable GetSchema(string collectionName, string[] restrictionValues) =>
            _connection.GetSchema(collectionName, restrictionValues);
    }
}