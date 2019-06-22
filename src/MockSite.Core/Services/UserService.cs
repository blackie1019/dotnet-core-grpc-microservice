#region

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Enums;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;

#endregion

namespace MockSite.Core.Services
{
    public class UserService : IUserService
    {
        private readonly ConnectDb _useDb;
        private readonly IRepository _userRepository;
        private readonly IMongoRepository _mongoUserRepository;
        private readonly IRedisRepository _redisUserRepository;

        public UserService(
            IConfiguration config,
            IRepository repository,
            IMongoRepository mongoUserRepository = null,
            IRedisRepository redisRepository = null
        )
        {
            _useDb = (ConnectDb) int.Parse(config.GetSection(DbConnectionConst.UseDbKey).Value);
            _userRepository = repository;
            _mongoUserRepository = mongoUserRepository;
            _redisUserRepository = redisRepository;
        }

        public async Task Create(UserDto user)
        {
            switch (_useDb)
            {
                case ConnectDb.MariaDb:
                    await _userRepository.Create(user);
                    break;
                case ConnectDb.Mongo:
                    await _mongoUserRepository.Create(user);
                    break;
            }
            if (_redisUserRepository != null)
            {
                await _redisUserRepository.Create(user);
            }
        }

        public async Task Update(UserDto user)
        {
            switch (_useDb)
            {
                case ConnectDb.MariaDb:
                    await _userRepository.Update(user);
                    break;
                case ConnectDb.Mongo:
                    await _mongoUserRepository.Update(user);
                    break;
            }
            if (_redisUserRepository != null)
            {
                await _redisUserRepository.Update(user);
            }
        }

        public async Task Delete(int id)
        {
            switch (_useDb)
            {
                case ConnectDb.MariaDb:
                    await _userRepository.Delete(id);
                    break;
                case ConnectDb.Mongo:
                    await _mongoUserRepository.Delete(id);
                    break;
            }
            if (_redisUserRepository != null)
            {
                await _redisUserRepository.Delete(id);
            }
        }

        public async Task<UserEntity> GetById(int id)
        {
            UserEntity user = null;
            if (_redisUserRepository != null)
            {
                user = await _redisUserRepository.GetById(id);
                if (user != null) return user;
            }
            switch (_useDb)
            {
                case ConnectDb.MariaDb:
                    user = await _userRepository.GetById(id);
                    break;
                case ConnectDb.Mongo:
                    user = await _mongoUserRepository.GetById(id);
                    break;
            }
            if (user != null && _redisUserRepository != null)
            {
                await _redisUserRepository.Create(ConvertEntityToDto(user));
            }
            return user;
        }

        public async Task<IEnumerable<UserEntity>> GetAll()
        {
            List<UserEntity> entities = null;

            switch (_useDb)
            {
                case ConnectDb.MariaDb:
                    entities = (await _userRepository.GetAll()).ToList();
                    break;
                case ConnectDb.Mongo:
                    entities = (await _mongoUserRepository.GetAll()).ToList();
                    break;
            }
            if (entities == null) return null;
            if (_redisUserRepository == null) return entities;

            await _redisUserRepository.DeleteAll();
            foreach (var entity in entities)
            {
                await _redisUserRepository.Create(ConvertEntityToDto(entity));
            }
            return entities;
        }

        private UserDto ConvertEntityToDto(UserEntity entity)
        {
            return new UserDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Email = entity.Email,
                Name = entity.Name,
                Password = entity.Password
            };
        }
    }
}