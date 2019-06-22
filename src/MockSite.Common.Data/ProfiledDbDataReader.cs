#region

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace MockSite.Common.Data
{
    public class ProfiledDbDataReader : DbDataReader
    {
        private readonly Action _callback;

        public ProfiledDbDataReader(DbDataReader reader, Action callback)
        {
            WrappedReader = reader;
            this._callback = callback;
        }

        public override int Depth => WrappedReader.Depth;

        public override int FieldCount => WrappedReader.FieldCount;

        public override bool HasRows => WrappedReader.HasRows;

        public override bool IsClosed => WrappedReader.IsClosed;

        public override int RecordsAffected => WrappedReader.RecordsAffected;

        public DbDataReader WrappedReader { get; }

        public override object this[string name] => WrappedReader[name];

        public override object this[int ordinal] => WrappedReader[ordinal];

        public override bool GetBoolean(int ordinal) => WrappedReader.GetBoolean(ordinal);

        public override byte GetByte(int ordinal) => WrappedReader.GetByte(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) =>
            WrappedReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

        public override char GetChar(int ordinal) => WrappedReader.GetChar(ordinal);

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) =>
            WrappedReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

        public new DbDataReader GetData(int ordinal) => WrappedReader.GetData(ordinal);

        public override string GetDataTypeName(int ordinal) => WrappedReader.GetDataTypeName(ordinal);

        public override DateTime GetDateTime(int ordinal) => WrappedReader.GetDateTime(ordinal);

        public override decimal GetDecimal(int ordinal) => WrappedReader.GetDecimal(ordinal);

        public override double GetDouble(int ordinal) => WrappedReader.GetDouble(ordinal);

        public override System.Collections.IEnumerator GetEnumerator() =>
            ((System.Collections.IEnumerable) WrappedReader).GetEnumerator();

        public override Type GetFieldType(int ordinal) => WrappedReader.GetFieldType(ordinal);

        public override T GetFieldValue<T>(int ordinal) => WrappedReader.GetFieldValue<T>(ordinal);

        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) =>
            WrappedReader.GetFieldValueAsync<T>(ordinal, cancellationToken);

        public override float GetFloat(int ordinal) => WrappedReader.GetFloat(ordinal);

        public override Guid GetGuid(int ordinal) => WrappedReader.GetGuid(ordinal);

        public override short GetInt16(int ordinal) => WrappedReader.GetInt16(ordinal);

        public override int GetInt32(int ordinal) => WrappedReader.GetInt32(ordinal);

        public override long GetInt64(int ordinal) => WrappedReader.GetInt64(ordinal);

        public override string GetName(int ordinal) => WrappedReader.GetName(ordinal);

        public override int GetOrdinal(string name) => WrappedReader.GetOrdinal(name);

        public override string GetString(int ordinal) => WrappedReader.GetString(ordinal);

        public override object GetValue(int ordinal) => WrappedReader.GetValue(ordinal);

        public override int GetValues(object[] values) => WrappedReader.GetValues(values);

        public override bool IsDBNull(int ordinal) => WrappedReader.IsDBNull(ordinal);

        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) =>
            WrappedReader.IsDBNullAsync(ordinal, cancellationToken);

        public override bool NextResult() => WrappedReader.NextResult();

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken) =>
            WrappedReader.NextResultAsync(cancellationToken);

        public override bool Read() => WrappedReader.Read();

        public override Task<bool> ReadAsync(CancellationToken cancellationToken) =>
            WrappedReader.ReadAsync(cancellationToken);

        public override void Close()
        {
            // reader can be null when we're not profiling, but we've inherited from ProfiledDbCommand and are returning a
            // an unwrapped reader from the base command
            WrappedReader?.Close();
            _callback?.Invoke();
        }

        public override DataTable GetSchemaTable() => WrappedReader.GetSchemaTable();

        protected override void Dispose(bool disposing)
        {
            // reader can be null when we're not profiling, but we've inherited from ProfiledDbCommand and are returning a
            // an unwrapped reader from the base command
            WrappedReader?.Dispose();
            base.Dispose(disposing);
        }
    }
}