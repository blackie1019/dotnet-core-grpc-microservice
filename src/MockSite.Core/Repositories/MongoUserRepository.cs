#region

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using MockSite.Core.Utilities;
using MongoDB.Driver;

#endregion

namespace MockSite.Core.Repositories
{
    public class MongoUserRepository : IMongoRepository
    {
        private readonly string _dbString;
        private readonly string _collectionString;
        private readonly MongoClient _connection;

        public MongoUserRepository(ILogger<MongoUserRepository> logger, IConfiguration config)
        {
            var connectionString = config.GetSection(DbConnectionConst.MongoTestKey).Value;
            _dbString = config.GetSection(DbConnectionConst.MongoDbKey).Value;
            _collectionString = config.GetSection(DbConnectionConst.MongoCollectionKey).Value;
            _connection = new MongoClient(connectionString);
            logger.LogInformation("log init");
            Mapper.Initialize(cfg => cfg.AddProfile<MongoMapperProfile>());
        }

        public async Task Create(UserDto userDto)
        {
            var db = _connection.GetDatabase(_dbString);
            var collection = db.GetCollection<UserDto>(_collectionString);
            await collection.InsertOneAsync(userDto);
        }

        public async Task Update(UserDto userDto)
        {
            var db = _connection.GetDatabase(_dbString);
            var collection = db.GetCollection<UserDto>(_collectionString);
            var update = new UpdateDefinitionBuilder<UserDto>()
                .Set(i => i.Name, userDto.Name)
                .Set(i => i.Email, userDto.Email);
            await collection.UpdateOneAsync(i => i.Id == userDto.Id, update);
        }

        public async Task Delete(int id)
        {
            var db = _connection.GetDatabase(_dbString);
            var collection = db.GetCollection<UserDto>(_collectionString);
            await collection.DeleteOneAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<UserEntity>> GetAll()
        {
            var db = _connection.GetDatabase(_dbString);
            var collection = db.GetCollection<UserDto>(_collectionString);
            var documents = await collection.Find(_ => true).ToListAsync();
            return Mapper.Map<List<UserDto>, List<UserEntity>>(documents);
        }

        public async Task<UserEntity> GetById(int id)
        {
            var db = _connection.GetDatabase(_dbString);
            var collection = db.GetCollection<UserDto>(_collectionString);
            var filter = Builders<UserDto>.Filter.Eq("Id", id);
            var document = await collection.Find(filter).FirstOrDefaultAsync();
            return Mapper.Map<UserDto, UserEntity>(document);
        }
    }
}