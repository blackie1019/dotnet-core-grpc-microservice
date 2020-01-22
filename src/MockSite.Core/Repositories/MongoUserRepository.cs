#region

using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using MockSite.Core.Utilities;
using MongoDB.Driver;

#endregion

namespace MockSite.Core.Repositories
{
    public class MongoUserRepository : IMongoUserRepository
    {
        private readonly string _collectionString;
        private readonly MongoClient _connection;
        private readonly string _dbString;

        public MongoUserRepository(ILogger<MongoUserRepository> logger, IConfiguration consul)
        {
            var connectionString = consul[DbConnectionConst.MongoTestKey];
            _dbString = consul[DbConnectionConst.MongoDbKey];
            _collectionString = consul[DbConnectionConst.MongoCollectionKey];
            _connection = new MongoClient(connectionString);
            logger.LogInformation("log init");
            Mapper.Initialize(cfg => cfg.AddProfile<MongoMapperProfile>());
        }

        public async Task<int> Create(UserEntity userEntity)
        {
            var collection = GetCollection();
            await collection.InsertOneAsync(userEntity);

            return userEntity.Id;
        }

        public async Task Update(UserEntity userEntity)
        {
            var collection = GetCollection();
            var update = new UpdateDefinitionBuilder<UserEntity>()
                .Set(i => i.Name, userEntity.Name)
                .Set(i => i.Email, userEntity.Email);
            await collection.UpdateOneAsync(i => i.Id == userEntity.Id, update);
        }

        public async Task Delete(int id)
        {
            var collection = GetCollection();
            await collection.DeleteOneAsync(a => a.Id == id);
        }

        public async Task<UserEntity[]> GetAll()
        {
            var collection = GetCollection();
            var documents = (await collection.FindAsync(_ => true)).ToEnumerable().ToArray();

            return documents;
        }

        public async Task<UserEntity> GetById(int id)
        {
            var collection = GetCollection();
            var filter = Builders<UserEntity>.Filter.Eq("Id", id);
            var document = await collection.Find(filter).FirstOrDefaultAsync();

            return document;
        }

        private IMongoCollection<UserEntity> GetCollection()
        {
            var db = _connection.GetDatabase(_dbString);
            var collection = db.GetCollection<UserEntity>(_collectionString);

            return collection;
        }

        public async Task<UserEntity[]> GetByCondition(string code = null, string name = null, string email = null)
        {
            var collection = GetCollection();
            var builder = Builders<UserEntity>.Filter;
            var filter = builder.Empty;

            if (code.HasValue())
            {
                filter = builder.Eq(u => u.Code, code);
            }

            if (name.HasValue())
            {
                if (filter != builder.Empty)
                {
                    filter &= builder.Eq(u => u.Name, name);
                }
                else
                {
                    filter = builder.Eq(u => u.Name, name);
                }
            }
            
            if (email.HasValue())
            {
                if (filter != builder.Empty)
                {
                    filter &= builder.Eq(u => u.Email, email);
                }
                else
                {
                    filter = builder.Eq(u => u.Email, email);
                }
            }

            var document = (await collection.FindAsync(filter)).ToEnumerable().ToArray();

            return document;
        }
    }
}