#region

using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace MockSite.Core.DTOs
{
    public class UserDto
    {
        [BsonId] public int Id { get; }

        [BsonElement] public string Code { get; }

        [BsonElement] public string Email { get; }

        [BsonElement] public string Name { get; }

        [BsonElement] public string Password { get; }

        
        
        public UserDto(int id, string name, string email)
        :this(id,string.Empty,name,email,string.Empty)
        {
            
        }
        
        public UserDto(string code, string name, string email, string password)
        :this(0,code,name,email,password)
        {
        }
        
        public UserDto(int id, string code, string name, string email, string password)
        {
            Id = id;
            Code = code;
            Email = email;
            Name = name;
            Password = password;
        }
    }
}