#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Data.Utilities;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using StackExchange.Redis;
using LuaScript = MockSite.Core.Lua.LuaScript;

#endregion

namespace MockSite.Core.Repositories
{
    public class RedisUserRepository : IRedisRepository
    {
        private const string Prefix = "Users:";
        private readonly LuaScript _luaScript;

        public RedisUserRepository(IConfiguration config, RedisConnectHelper redisConn)
        {
            var conn = config.GetSection(DbConnectionConst.RedisKey).Value;
            _luaScript = new LuaScript(redisConn.CreateConnection(conn), conn);
        }

        public async Task Create(UserDto userDto)
        {
            await _luaScript.ExecLuaScript(
                LuaScript.Create,
                new RedisKey[] {Prefix + userDto.Id},
                new RedisValue[] {userDto.Id, userDto.Code, userDto.Email, userDto.Name, userDto.Password}
            );
        }

        public async Task Update(UserDto userDto)
        {
            await _luaScript.ExecLuaScript(
                LuaScript.Update,
                new RedisKey[] {Prefix + userDto.Id},
                new RedisValue[] {userDto.Email, userDto.Name}
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

        public Task<IEnumerable<UserEntity>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public async Task<UserEntity> GetById(int id)
        {
            var data = (string[]) await _luaScript.ExecLuaScript(
                LuaScript.GetById,
                new RedisKey[] {Prefix + id}
            );
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