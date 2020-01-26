using MongoDB.Bson.Serialization.Attributes;

namespace MockSite.Core.DTOs
{
    public class LanguageDto
    {
        [BsonId]
        public string LangCode { get;}

        [BsonElement]
        public string DisplayValue { get;}

        public LanguageDto(string langCode, string displayValue)
        {
            this.LangCode = langCode;
            this.DisplayValue = displayValue;
        }

    }
}