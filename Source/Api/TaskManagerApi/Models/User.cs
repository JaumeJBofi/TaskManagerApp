using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskManagerApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; } 
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }        
        public string? RefreshToken {get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
