using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MockSite.Core.Entities
{
    public class LocalizationEntity
    {
        [BsonId]
        public string DisplayKey { get; set; }

        [BsonElement]
        public IEnumerable<LanguageEntity> LanguageSets { get; set; }
    }
}