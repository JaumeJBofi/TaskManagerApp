﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskManagerApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } 
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }        
        public string? RefreshToken {get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
