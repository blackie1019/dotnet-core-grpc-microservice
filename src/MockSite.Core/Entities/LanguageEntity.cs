using MongoDB.Bson.Serialization.Attributes;

namespace MockSite.Core.Entities
{
    public class LanguageEntity
    {
        [BsonElement]
        public string LangCode { get; set; }

        [BsonElement]
        public string DisplayValue { get; set; }
    }
}