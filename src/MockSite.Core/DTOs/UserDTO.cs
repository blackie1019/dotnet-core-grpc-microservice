#region

using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace MockSite.Core.DTOs
{
    public class UserDto
    {
        [BsonId] public int Id { get; set; }

        [BsonElement] public string Code { get; set; }

        [BsonElement] public string Email { get; set; }

        [BsonElement] public string Name { get; set; }

        [BsonElement] public string Password { get; set; }
    }
}