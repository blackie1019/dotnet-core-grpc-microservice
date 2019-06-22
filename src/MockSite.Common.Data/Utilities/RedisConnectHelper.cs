#region

using StackExchange.Redis;

#endregion

namespace MockSite.Common.Data.Utilities
{
    public class RedisConnectHelper
    {
        public ConnectionMultiplexer CreateConnection(string conn)
        {
            return ConnectionMultiplexer.Connect(conn);
        }
    }
}