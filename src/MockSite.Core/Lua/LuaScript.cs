#region

using System.Threading.Tasks;
using StackExchange.Redis;

#endregion

namespace MockSite.Core.Lua
{
    public class LuaScript
    {
        public const string Create = @"
            local targetKey = KEYS[1]
            local id = tonumber(ARGV[1])
            local code = ARGV[2]
            local email = ARGV[3]
            local name = ARGV[4]
            local password = ARGV[5]

            return redis.call('HSET', targetKey, 'Id', id, 'Code', code, 'Email', email, 'Name', name, 'Password', password)";

        public const string Update = @"
            local targetKey = KEYS[1]
            local email = ARGV[1]
            local name = ARGV[2]

            return redis.call('HSET', targetKey, 'Email', email, 'Name', name)";

        public const string DeleteAll = @"
            local targetKey = KEYS[1]
            local matches = redis.call('KEYS', targetKey .. '*')

            local result = 0
            for _,key in ipairs(matches) do
                result = result + redis.call('DEL', key)
            end

            return result";

        public const string GetById = @"
            local targetKey = KEYS[1]
            return redis.call('HGETALL', targetKey)";

        private readonly ConnectionMultiplexer _redis;
        private readonly string _redisConn;

        public LuaScript(ConnectionMultiplexer redis, string conn)
        {
            _redis = redis;
            _redisConn = conn;
        }

        public LoadedLuaScript ScriptObject(string luaScript)
        {
            return StackExchange.Redis.LuaScript
                .Prepare(luaScript)
                .Load(_redis.GetServer(_redisConn));
        }

        public async Task<RedisResult> ExecLuaScript(
            string luaScript,
            RedisKey[] redisKey,
            RedisValue[] redisValue = null
        )
        {
            return await _redis.GetDatabase().ScriptEvaluateAsync(
                ScriptObject(luaScript).Hash,
                redisKey,
                redisValue
            );
        }
    }
}