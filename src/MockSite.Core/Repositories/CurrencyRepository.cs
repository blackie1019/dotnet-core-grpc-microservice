using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using StackExchange.Redis;
using LuaScript = MockSite.Core.Lua.LuaScript;

namespace MockSite.Core.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private const string CurrencyKeyName = "CurrencyAll";

        private const string GetAllCurrencies = @"
            local currencyKey = KEYS[1]

            return redis.call('HGETALL', KEYS[1])";

        private const string GetTtlCurrency = @"
            local currencyKey = KEYS[1]

            local isExist = redis.call('Exists',currencyKey)

            if isExist == 0
            then 
                redis.call('SET', currencyKey,'20')
                redis.call('EXPIRE', currencyKey,60)
            end

            return redis.call('GET', currencyKey)";

        private readonly IDatabase _db;

        private readonly LuaScript _luaScript;

        public CurrencyRepository(IConfiguration config)
        {
            var conn = config[DbConnectionConst.RedisKey];
            var redis = ConnectionMultiplexer.Connect(conn);
            _db = redis.GetDatabase();
            _luaScript = new LuaScript(redis, conn);
        }

        public async Task Modify(CurrencyDto currencyDto)
        {
            HashEntry[] hashEntries =
            {
                new HashEntry(currencyDto.CurrencyCode, currencyDto.CurrencyRate)
            };

            await _db.HashSetAsync(CurrencyKeyName, hashEntries);
        }

        public async Task Delete(string code)
        {
            await _db.HashDeleteAsync(CurrencyKeyName, code);
        }

        public async Task<IEnumerable<CurrencyEntity>> GetCurrencyAll()
        {
            var currencies = new List<CurrencyEntity>();

            var data = (string[]) await _luaScript.ExecLuaScript(
                GetAllCurrencies,
                new RedisKey[] {"CurrencyAll"}
            );

            for (var pos = 0; pos < data.Length; pos += 2)
            {
                currencies.Add(new CurrencyEntity
                {
                    CurrencyCode = data[pos],
                    CurrencyRate = data[pos + 1]
                });
            }

            return currencies;
        }

        public async Task<IEnumerable<CurrencyEntity>> GetTtlCurrencies()
        {
            var currencies = new List<CurrencyEntity>();

            var data = (string[]) await _luaScript.ExecLuaScript(
                GetTtlCurrency,
                new RedisKey[] {"TtlCurrency"}
            );

            currencies.Add(new CurrencyEntity
            {
                CurrencyCode = "TtlCurrency",
                CurrencyRate = data[0]
            });

            return currencies;
        }
    }
}