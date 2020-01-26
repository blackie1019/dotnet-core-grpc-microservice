using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using MongoDB.Driver;

namespace MockSite.Core.Repositories
{
    public class LocalizationRepository : ILocalizationRepository
    {
        private const string CollectName = "Localization";
        private readonly IMongoDatabase _db;
        public LocalizationRepository(IConfiguration config)
        {
            var connectionString = config[DbConnectionConst.MongoTestKey];
            var dbString = config[DbConnectionConst.MongoDbKey];
            var connection = new MongoClient(connectionString);
            _db = connection.GetDatabase(dbString);
        }

        public async Task Insert(LocalizationEntity localizationEntity)
        {
            var collection = _db.GetCollection<LocalizationEntity>(CollectName);
            await collection.InsertOneAsync(localizationEntity);
        }

        public async Task Update(LocalizationEntity localizationEntity)
        {
            var languageSet = localizationEntity.LanguageSets.First();

            var basicFilter = Builders<LocalizationEntity>.Filter.Eq(l => l.DisplayKey, localizationEntity.DisplayKey);
            var elementFilter =
                Builders<LocalizationEntity>.Filter.ElemMatch(l => l.LanguageSets,
                    ls => ls.LangCode == languageSet.LangCode);

            FilterDefinition<LocalizationEntity> filter;
            UpdateDefinition<LocalizationEntity> update;
            
            var collection = _db.GetCollection<LocalizationEntity>(CollectName);
            var document = collection.FindAsync(Builders<LocalizationEntity>.Filter.And(basicFilter,elementFilter)).Result.Current;
            
            if (!document.Any())
            {
                filter = basicFilter;

                update = Builders<LocalizationEntity>.Update.Push(l => l.LanguageSets,
                    languageSet);
            }
            else
            {
                filter = Builders<LocalizationEntity>.Filter.And(basicFilter, elementFilter);

                update = Builders<LocalizationEntity>.Update.Set(l => l.LanguageSets.ElementAt(-1),
                    languageSet);
            }

            await collection.UpdateOneAsync(filter, update);
        }

        public async Task Delete(string displayKey)
        {
            var collection = _db.GetCollection<LocalizationEntity>(CollectName);
            await collection.DeleteOneAsync(a => a.DisplayKey == displayKey);
        }

        public async Task<LocalizationEntity> GetByCode(string displayKey)
        {
            var collection = _db.GetCollection<LocalizationEntity>(CollectName);
            var filter = Builders<LocalizationEntity>.Filter.Eq("DisplayKey", displayKey);

            var document = await collection.Find(filter).FirstOrDefaultAsync();

            return document;
        }

        public async Task<IEnumerable<LocalizationEntity>> GetAll()
        {
            var collection = _db.GetCollection<LocalizationEntity>(CollectName);
            var documents = await collection.Find(_ => true).ToListAsync();

            return documents;
        }
    }
}