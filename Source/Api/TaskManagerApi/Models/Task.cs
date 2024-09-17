using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TaskManagerApi.Models
{
    public enum TASK_STATUS
    {
        PENDING,
        INPROGRESS,
        COMPLETED
    }
    public class UserTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        [BsonRepresentation(BsonType.Int32)]
        public TASK_STATUS Status { get; set; }
        public required string UserId { get; set; }
    }
}
