using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MockSite.Core.DTOs
{
    public class LocalizationDto
    {
        [BsonId]
        public string DisplayKey { get;}

        [BsonElement]
        public IEnumerable<LanguageDto> LanguageSets
        {
            get { return _languageSets ?? (_languageSets = new LanguageDto[0]); }
        }

        private IEnumerable<LanguageDto> _languageSets;

        public LocalizationDto(string displayKey, IEnumerable<LanguageDto> languageSets)
        {
            DisplayKey = displayKey;

            _languageSets = languageSets;

        }

    }
}