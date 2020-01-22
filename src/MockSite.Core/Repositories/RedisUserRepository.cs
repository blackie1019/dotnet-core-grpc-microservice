#region

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Data.Utilities;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using StackExchange.Redis;
using LuaScript = MockSite.Core.Lua.LuaScript;

#endregion

namespace MockSite.Core.Repositories
{
    public class RedisUserRepository : IRedisUserRepository
    {
        private const string Prefix = "Users:";
        private readonly LuaScript _luaScript;

        public RedisUserRepository(IConfiguration config, RedisConnectHelper redisConn)
        {
            var conn = config[DbConnectionConst.RedisKey];
            _luaScript = new LuaScript(redisConn.CreateConnection(conn), conn);
        }

        public async Task<int> Create(UserEntity userEntity)
        {
            await _luaScript.ExecLuaScript(
                LuaScript.Create,
                new RedisKey[] {Prefix + userEntity.Id},
                new RedisValue[] {userEntity.Id, userEntity.Code, userEntity.Email, userEntity.Name, userEntity.Password}
            );

            return userEntity.Id;
        }

        public async Task Update(UserEntity userEntity)
        {
            await _luaScript.ExecLuaScript(
                LuaScript.Update,
                new RedisKey[] {Prefix + userEntity.Id},
                new RedisValue[] {userEntity.Email, userEntity.Name}
            );
        }

        public async Task Delete(int id)
        {
            await _luaScript.ExecLuaScript(LuaScript.DeleteAll, new RedisKey[] {Prefix + id});
        }

        public async Task DeleteAll()
        {
            await _luaScript.ExecLuaScript(LuaScript.DeleteAll, new RedisKey[] {Prefix});
        }

        public Task<UserEntity[]> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<UserEntity> GetById(int id)
        {
            var data = (string[]) await _luaScript.ExecLuaScript(
                LuaScript.GetById,
                new RedisKey[] {Prefix + id}
            );

            if (data.Length == 0)
                return null;

            return new UserEntity
            {
                Id = id,
                Code = data[3],
                Email = data[5],
                Name = data[7],
                Password = data[9]
            };
        }
    }
}