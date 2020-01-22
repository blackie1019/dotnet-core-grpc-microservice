using MongoDB.Bson.Serialization.Attributes;

namespace MockSite.Core.Entities
{
    public class UserEntity
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement]
        public string Code { get; set; }

        [BsonElement]
        public string Email { get; set; }

        [BsonElement]
        public string Name { get; set; }

        [BsonElement]
        public string Password { get; set; }
    }
}