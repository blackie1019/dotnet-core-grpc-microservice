#region

using System;
using System.Collections;
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
            this.WrappedReader = reader;
            _callback = callback;
        }

        public override int Depth
        {
            get { return this.WrappedReader.Depth; }
        }

        public override int FieldCount
        {
            get { return this.WrappedReader.FieldCount; }
        }

        public override bool HasRows
        {
            get { return this.WrappedReader.HasRows; }
        }

        public override bool IsClosed
        {
            get { return this.WrappedReader.IsClosed; }
        }

        public override int RecordsAffected
        {
            get { return this.WrappedReader.RecordsAffected; }
        }

        public DbDataReader WrappedReader { get; }

        public override object this[string name]
        {
            get { return this.WrappedReader[name]; }
        }

        public override object this[int ordinal]
        {
            get { return this.WrappedReader[ordinal]; }
        }

        public override bool GetBoolean(int ordinal)
        {
            return this.WrappedReader.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return this.WrappedReader.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return this.WrappedReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return this.WrappedReader.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return this.WrappedReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public new DbDataReader GetData(int ordinal)
        {
            return this.WrappedReader.GetData(ordinal);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return this.WrappedReader.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return this.WrappedReader.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return this.WrappedReader.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return this.WrappedReader.GetDouble(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            return ((IEnumerable) this.WrappedReader).GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            return this.WrappedReader.GetFieldType(ordinal);
        }

        public override T GetFieldValue<T>(int ordinal)
        {
            return this.WrappedReader.GetFieldValue<T>(ordinal);
        }

        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
        {
            return this.WrappedReader.GetFieldValueAsync<T>(ordinal, cancellationToken);
        }

        public override float GetFloat(int ordinal)
        {
            return this.WrappedReader.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return this.WrappedReader.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return this.WrappedReader.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return this.WrappedReader.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return this.WrappedReader.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return this.WrappedReader.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return this.WrappedReader.GetOrdinal(name);
        }

        public override string GetString(int ordinal)
        {
            return this.WrappedReader.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return this.WrappedReader.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return this.WrappedReader.GetValues(values);
        }

        public override bool IsDBNull(int ordinal)
        {
            return this.WrappedReader.IsDBNull(ordinal);
        }

        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
        {
            return this.WrappedReader.IsDBNullAsync(ordinal, cancellationToken);
        }

        public override bool NextResult()
        {
            return this.WrappedReader.NextResult();
        }

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return this.WrappedReader.NextResultAsync(cancellationToken);
        }

        public override bool Read()
        {
            return this.WrappedReader.Read();
        }

        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return this.WrappedReader.ReadAsync(cancellationToken);
        }

        public override void Close()
        {
            // reader can be null when we're not profiling, but we've inherited from ProfiledDbCommand and are returning a
            // an unwrapped reader from the base command
            this.WrappedReader?.Close();
            _callback?.Invoke();
        }

        public override DataTable GetSchemaTable()
        {
            return this.WrappedReader.GetSchemaTable();
        }

        protected override void Dispose(bool disposing)
        {
            // reader can be null when we're not profiling, but we've inherited from ProfiledDbCommand and are returning a
            // an unwrapped reader from the base command
            this.WrappedReader?.Dispose();
            base.Dispose(disposing);
        }
    }
}